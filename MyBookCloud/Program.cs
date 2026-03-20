using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBookCloud.Application.Configurations;
using MyBookCloud.Consumers;
using MyBookCloud.Hubs;
using MyBookCloud.Infrastructure.Configurations;
using MyBookCloud.Persistence;
using MyBookCloud.Persistence.Configurations;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<BookEnrichedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["Bus:RabbitMqHost"], builder.Configuration["Bus:RabbitMqVirtualHost"], h =>
        {
            h.Username(builder.Configuration["Bus:RabbitMqUsername"]);
            h.Password(builder.Configuration["Bus:RabbitMqPassword"]);
        });

        cfg.ConfigureEndpoints(context);
    });
});

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key is not configured!");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        // Для SignalR токен передається клієнтом у query string (`access_token`)
        // (особливо для WebSocket транспорту). Без цього хаб буде неавторизований,
        // а події до клієнта не дійдуть.
        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var accessToken = ctx.Request.Query["access_token"].ToString();
                var path = ctx.HttpContext.Request.Path;

                if (!string.IsNullOrWhiteSpace(accessToken) && path.StartsWithSegments("/hubs/books"))
                {
                    ctx.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MyBookCloudDbContext>();

    // Спробуємо 10 разів з паузою в 2 секунди
    for (int i = 0; i < 10; i++)
    {
        try
        {
            context.Database.Migrate();
            Console.WriteLine("---> Database is ready!");
            break; // Перемога, виходимо з циклу
        }
        catch (Exception)
        {
            Console.WriteLine("---> Database is not ready yet, retrying...");
            Thread.Sleep(2000); // Чекаємо 2 секунди
        }
    }
}

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BookHub>("/hubs/books");

app.Run();
