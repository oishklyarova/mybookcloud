using MyBookCloud.Business.SeedWork;

namespace MyBookCloud.Business.Books
{
    public interface IBookRepository : IRepository<BookEntity>
    {
        IQueryable<BookEntity> GetAll();
    }
}
