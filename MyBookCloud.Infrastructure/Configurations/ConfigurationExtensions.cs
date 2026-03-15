using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBookCloud.Application.Connectors;
using MyBookCloud.Infrastructure.GoogleBookApi;

namespace MyBookCloud.Infrastructure.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddTransient<IGoogleBookApiConnector, GoogleBookApiConnector>();
        }
    }
}
