using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyBookCloud.Configurations;
using MyBookCloud.Core.Api.Consumers;
using MyBookCloud.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure(builder.Configuration);
builder.Services.AddDbContext<MyBookCloudDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.ConfigureMapper();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<BookCreatedConsumer>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
