using MassTransit;
using MyBookCloud.Application.Services;
using MyBookCloud.Common.Messages;

namespace MyBookCloud.Worker.Consumers
{
    public class BookCreatedConsumer : IConsumer<IBookCreatedMessage>
    {
        private readonly ILogger<BookCreatedConsumer> _logger;
        private readonly IBookService _bookService;

        public BookCreatedConsumer(ILogger<BookCreatedConsumer> logger,
            IBookService bookService)
        {
            _logger = logger;
            _bookService = bookService;
        }

        public async Task Consume(ConsumeContext<IBookCreatedMessage> context)
        {
            _logger.LogInformation(" [*] Message received BookId: {id}, ISBN: {isbn} ", context.Message.BookId, context.Message.Isbn);

            await _bookService.EnrichBookDataAsync(context.Message.BookId, context.Message.Isbn, context.Message.UserId);
        }
    }
}
