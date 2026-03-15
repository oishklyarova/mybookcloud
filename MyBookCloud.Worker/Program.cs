using MassTransit;
using MyBookCloud.Application.Configurations;
using MyBookCloud.Persistence.Configurations;
using MyBookCloud.Infrastructure.Configurations;
using MyBookCloud.Worker.Consumers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

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

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

var host = builder.Build();
host.Run();
