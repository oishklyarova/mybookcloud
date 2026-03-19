using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MyBookCloud.Common.Messages;
using MyBookCloud.Hubs;

namespace MyBookCloud.Consumers
{
    public class BookEnrichedConsumer : IConsumer<IBookEnrichedMessage>
    {
        private readonly ILogger<BookEnrichedConsumer> _logger;
        private readonly IHubContext<BookHub> _hubContext;

        public BookEnrichedConsumer(ILogger<BookEnrichedConsumer> logger, IHubContext<BookHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IBookEnrichedMessage> context)
        {
            _logger.LogInformation("Book enriched: {BookId}", context.Message.BookId);

            _logger.LogInformation("Sending BookEnriched to user: {UserId}", context.Message.UserId);

            await _hubContext.Clients
                .User(context.Message.UserId.ToString())
                .SendAsync("BookEnriched", new
                {
                    bookId = context.Message.BookId,
                    coverThumbnailUrl = context.Message.CoverThumbnailUrl,
                    pageCount = context.Message.PageCount
                });
        }
    }
}

