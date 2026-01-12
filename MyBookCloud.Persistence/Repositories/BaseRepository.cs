using MyBookCloud.Business.SeedWork;

namespace MyBookCloud.Persistence.Repositories
{
    public class BaseRepository : IRepository<BaseEntity>
    {
        protected readonly MyBookCloudDbContext context;

        public BaseRepository(MyBookCloudDbContext context)
        {
            this.context = context;
        }
    }
}
