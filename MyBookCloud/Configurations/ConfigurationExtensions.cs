using AutoMapper;
using MyBookCloud.Business.Books;
using MyBookCloud.Business.SeedWork;
using MyBookCloud.Core.Api.AutoMapperProfiles;
using MyBookCloud.Core.Api.Connectors;
using MyBookCloud.Core.Api.Connectors.Impl;
using MyBookCloud.Core.Api.Services;
using MyBookCloud.Core.Api.Services.Impl;
using MyBookCloud.Persistence;
using MyBookCloud.Persistence.Repositories;

namespace MyBookCloud.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection Configure(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddScoped<IUnitOfWork<MyBookCloudDbContext>, UnitOfWork<MyBookCloudDbContext>>()
                           .AddTransient<IBookRepository, BookRepository>()
                           .AddTransient<IGoogleBookApiConnector, GoogleBookApiConnector>()
                           .AddScoped<IBookService, BookService>();
        }

        public static IServiceCollection ConfigureMapper(this IServiceCollection services)
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
