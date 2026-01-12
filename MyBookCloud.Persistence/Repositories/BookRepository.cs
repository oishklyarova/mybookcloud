using MyBookCloud.Business.Books;

namespace MyBookCloud.Persistence.Repositories
{
    public class BookRepository : BaseRepository<BookEntity>, IBookRepository
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
