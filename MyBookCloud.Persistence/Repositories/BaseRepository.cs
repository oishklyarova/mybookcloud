using Microsoft.EntityFrameworkCore;
using MyBookCloud.Business.SeedWork;

namespace MyBookCloud.Persistence.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly MyBookCloudDbContext context;

        public BaseRepository(MyBookCloudDbContext context)
        {
            this.context = context;
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }
    }
}
