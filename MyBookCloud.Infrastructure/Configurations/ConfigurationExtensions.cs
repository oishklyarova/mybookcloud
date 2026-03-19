using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBookCloud.Application.Connectors;
using MyBookCloud.Application.Services;
using MyBookCloud.Infrastructure.CurrentUser;
using MyBookCloud.Infrastructure.GoogleBookApi;
using MyBookCloud.Infrastructure.Jwt;

namespace MyBookCloud.Infrastructure.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            services.AddHttpClient<IGoogleBookApiConnector, GoogleBookApiConnector>(client =>
            {
                client.BaseAddress = new Uri("https://www.googleapis.com");
                client.Timeout = TimeSpan.FromSeconds(30);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }

}
