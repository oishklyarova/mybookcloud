using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyBookCloud.Business.Books;
using MyBookCloud.Business.SeedWork;
using MyBookCloud.Core.Api.Connectors;
using MyBookCloud.Core.Api.Connectors.Impl;
using MyBookCloud.Core.Api.Messages;
using MyBookCloud.Persistence;

namespace MyBookCloud.Core.Api.Consumers
{
    public class BookCreatedConsumer : IConsumer<IBookCreatedMessage>
    {
        private readonly ILogger<BookCreatedConsumer> _logger;
        private readonly IGoogleBookApiConnector _googleBookApiConnector;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork<MyBookCloudDbContext> _unitOfWork;

        public BookCreatedConsumer(ILogger<BookCreatedConsumer> logger,
            IGoogleBookApiConnector googleBookApiConnector,
            IBookRepository bookRepository,
            IUnitOfWork<MyBookCloudDbContext> unitOfWork)
        {
            _logger = logger;
            _googleBookApiConnector = googleBookApiConnector;
            _bookRepository = bookRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<IBookCreatedMessage> context)
        {
            _logger.LogInformation(" [*] Message received BookId: {id}, ISBN: {isbn} ", context.Message.BookId, context.Message.Isbn);

            if (string.IsNullOrWhiteSpace(context.Message.Isbn))
            {
                _logger.LogInformation("ISBN is empty for BookId: {id}. Skipping Google Books lookup.", context.Message.BookId);
                return;
            }

            var volumeInfo = await _googleBookApiConnector.GetVolumeInfoAsync(context.Message.Isbn);
            if (volumeInfo == null)
            {
                _logger.LogInformation("Google Books returned no info for ISBN: {isbn}.", context.Message.Isbn);
                return;
            }

            var book = await _bookRepository.FindAsync(context.Message.BookId);
            if (book == null)
            {
                _logger.LogWarning("Book with Id {id} not found when trying to enrich from Google Books.", context.Message.BookId);
                return;
            }

            book.CoverThumbnailUrl = volumeInfo.ThumbnailUrl;
            book.PageCount = volumeInfo.PageCount;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
