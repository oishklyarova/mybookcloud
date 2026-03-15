using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using MyBookCloud.Business.Books;
using MyBookCloud.Business.SeedWork;
using MyBookCloud.Core.Api.Dto;
using MyBookCloud.Core.Api.Messages;
using MyBookCloud.Persistence;

namespace MyBookCloud.Core.Api.Services.Impl
{
    public class BookService : IBookService
    {
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork<MyBookCloudDbContext> _unitOfWork;
        private readonly IPublishEndpoint _publish;

        public BookService(IMapper mapper, IBookRepository bookRepository, IUnitOfWork<MyBookCloudDbContext> unitOfWork,
            IPublishEndpoint publish)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
            _publish = publish;
        }

        public async Task<List<BookData>> GetAllBooksAsync()
        {
            var books = await _bookRepository.GetAll().ToListAsync();
            return _mapper.Map<List<BookData>>(books);
        }

        public async Task<BookData> AddBookAsync(BookData bookData)
        {
            var bookEntity = _mapper.Map<BookEntity>(bookData);
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
    }
}

