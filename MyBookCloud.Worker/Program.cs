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
        cfg.UseMessageRetry(r =>
        {
            // 5 спроб: приблизно через 2с, 4с, 8с, 16с, 32с
            r.Exponential(5,
                TimeSpan.FromSeconds(2),    // Початкова пауза
                TimeSpan.FromSeconds(30),   // Максимальна пауза
                TimeSpan.FromSeconds(2));   // Крок збільшення
        });
        cfg.UseCircuitBreaker(cb =>
        {
            cb.TrackingPeriod = TimeSpan.FromMinutes(1); // Стежимо за помилками протягом хвилини
            cb.TripThreshold = 15;                       // Якщо 15% запитів падають...
            cb.ActiveThreshold = 10;                     // ...і було мінімум 10 запитів...
            cb.ResetInterval = TimeSpan.FromMinutes(5);  // ...закриваємо доступ на 5 хвилин
        });
    });
});

builder.Services.AddHttpContextAccessor();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Services.AddSerilog();

var host = builder.Build();
host.Run();
