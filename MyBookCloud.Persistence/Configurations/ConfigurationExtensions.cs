using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyBookCloud.Business.Books;
using MyBookCloud.Business.SeedWork;
using MyBookCloud.Persistence.Repositories;

namespace MyBookCloud.Persistence.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<MyBookCloudDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
            return services.AddScoped<IUnitOfWork<MyBookCloudDbContext>, UnitOfWork<MyBookCloudDbContext>>()
                           .AddTransient<IBookRepository, BookRepository>();
        }
    }
}
