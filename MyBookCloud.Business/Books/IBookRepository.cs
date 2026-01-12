using MyBookCloud.Business.SeedWork;
using MyBookCloud.Data.Entities;

namespace MyBookCloud.Business.Books
{
    public interface IBookRepository : IRepository<BookEntity>
    {
        IQueryable<BookEntity> GetAll();
    }
}
