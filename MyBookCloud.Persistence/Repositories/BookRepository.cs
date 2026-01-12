using MyBookCloud.Business.Books;
using MyBookCloud.Data.Entities;

namespace MyBookCloud.Persistence.Repositories
{
    public class BookRepository : BaseRepository, IBookRepository
    {
        public BookRepository(MyBookCloudDbContext context) : base(context)
        {
        }

        public IQueryable<BookEntity> GetAll()
        {
            return context.Books;
        }
    }
}
