using Microsoft.EntityFrameworkCore;
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

        public async Task<BookEntity?> FindAsync(Guid id)
        {
            var book = await context.Books
                .Where(b => b.Id == id)
                .SingleOrDefaultAsync();

            return book;
        }
    }
}
