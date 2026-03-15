using MassTransit;
using Microsoft.Extensions.Logging;
using MyBookCloud.Core.Api.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Core.Api.Consumers
{
    public class BookCreatedConsumer : IConsumer<IBookCreatedMessage>
    {
        private readonly ILogger<BookCreatedConsumer> logger;

        public BookCreatedConsumer(ILogger<BookCreatedConsumer> logger)
        {
            this.logger = logger;
        }

        public Task Consume(ConsumeContext<IBookCreatedMessage> context)
        {
            logger.LogInformation(" [*] Message received BookId: {id}, ISBN: {isbn} ", context.Message.BookId, context.Message.Isbn);
            return Task.CompletedTask;
        }
    }
}
