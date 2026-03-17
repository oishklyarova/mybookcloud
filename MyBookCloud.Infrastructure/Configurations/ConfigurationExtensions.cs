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
            return services.AddTransient<IGoogleBookApiConnector, GoogleBookApiConnector>()
                            .AddScoped<ICurrentUserService, CurrentUserService>()
                            .AddSingleton<IJwtTokenService, JwtTokenService>();
        }
    }
}
