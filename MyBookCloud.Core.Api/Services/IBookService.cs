using MyBookCloud.Core.Api.Dto;

namespace MyBookCloud.Core.Api.Services
{
    public interface IBookService
    {
        Task<List<BookData>> GetAllBooksAsync();
        Task<BookData> AddBookAsync(BookData bookData);
        Task<BookData?> UpdateBookAsync(Guid id, BookData bookData);
        Task<bool> DeleteBookAsync(Guid id);
    }
}

