using MyBookCloud.Application.Dto;

namespace MyBookCloud.Application.Services
{
    public interface IBookService
    {
        Task<List<BookData>> GetAllBooksAsync();
        Task<BookData> AddBookAsync(BookData bookData);
        Task<BookData?> UpdateBookAsync(Guid id, BookData bookData);
        Task<bool> DeleteBookAsync(Guid id);
        Task EnrichBookDataAsync(Guid bookId, string isbn, Guid userId);
    }
}

