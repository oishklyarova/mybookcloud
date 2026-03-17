using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyBookCloud.Application.Connectors;
using MyBookCloud.Application.Dto;
using MyBookCloud.Application.Services;
using MyBookCloud.Business.Books;
using MyBookCloud.Business.SeedWork;
using MyBookCloud.Common.Messages;
using MyBookCloud.Persistence;

namespace MyBookCloud.Application.Services.Impl
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork<MyBookCloudDbContext> _unitOfWork;
        private readonly IPublishEndpoint _publish;
        private readonly IGoogleBookApiConnector _googleConnector;
        private readonly ICurrentUserService _currentUser;

        public BookService(IMapper mapper, IBookRepository bookRepository, IUnitOfWork<MyBookCloudDbContext> unitOfWork,
            IPublishEndpoint publish, IGoogleBookApiConnector googleConnector, ICurrentUserService currentUser)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
            _publish = publish;
            _googleConnector = googleConnector;
            _currentUser = currentUser;
        }

        public async Task<List<BookData>> GetAllBooksAsync()
        {
            var userId = _currentUser.UserId ?? throw new InvalidOperationException("User is not authenticated.");

            var books = await _bookRepository
                .GetAll()
                .Where(x => x.CreatedById == userId)
                .OrderBy(x => x.Title)
                .ToListAsync();
            return _mapper.Map<List<BookData>>(books);
        }

        public async Task<BookData> AddBookAsync(BookData bookData)
        {
            var bookEntity = _mapper.Map<BookEntity>(bookData);
            bookEntity.CreatedById = _currentUser.UserId ?? throw new InvalidOperationException("User is not authenticated.");
            _bookRepository.Add(bookEntity);
            await _unitOfWork.SaveChangesAsync();
            await _publish.Publish<IBookCreatedMessage>(new
            {
                BookId = bookEntity.Id,
                bookEntity.Isbn
            });
            return _mapper.Map<BookData>(bookEntity);
        }

        public async Task<BookData?> UpdateBookAsync(Guid id, BookData bookData)
        {
            var bookEntity = await _bookRepository.GetByIdAsync(id);
            if (bookEntity == null)
            {
                return null;
            }

            _mapper.Map(bookData, bookEntity);
            _bookRepository.Update(bookEntity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<BookData>(bookEntity);
        }

        public async Task<bool> DeleteBookAsync(Guid id)
        {
            var bookEntity = await _bookRepository.GetByIdAsync(id);
            if (bookEntity == null)
            {
                return false;
            }

            _bookRepository.Delete(bookEntity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task EnrichBookDataAsync(Guid bookId, string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return;

            var volumeInfo = await _googleConnector.GetVolumeInfoAsync(isbn);

            var book = await _bookRepository.FindAsync(bookId);
            if (book == null) return;

            book.CoverThumbnailUrl = volumeInfo.ThumbnailUrl;
            book.PageCount = volumeInfo.PageCount;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}

