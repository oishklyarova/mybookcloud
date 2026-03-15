using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBookCloud.Application.AutoMapperProfiles;
using MyBookCloud.Application.Services;
using MyBookCloud.Application.Services.Impl;

namespace MyBookCloud.Application.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.ConfigureMapper();
            return services.AddTransient<IBookService, BookService>();
        }

        private static IServiceCollection ConfigureMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BookProfile>();
            });

            var mapper = mappingConfig.CreateMapper();
            return services.AddSingleton(mapper);
        }
    }
}
