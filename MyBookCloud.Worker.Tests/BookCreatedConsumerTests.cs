using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using MyBookCloud.Application.Services;
using MyBookCloud.Common.Messages;
using MyBookCloud.Worker.Consumers;

namespace MyBookCloud.Worker.Tests;

[TestClass]
public class BookCreatedConsumerTests
{
    [TestMethod]
    public async Task Consume_CallsBookServiceToEnrichBookData()
    {
        var bookId = Guid.NewGuid();
        var isbn = "978-555";
        var userId = Guid.NewGuid();

        var logger = new Mock<ILogger<BookCreatedConsumer>>();
        var bookService = new Mock<IBookService>(MockBehavior.Strict);
        bookService
            .Setup(x => x.EnrichBookDataAsync(bookId, isbn, userId))
            .Returns(Task.CompletedTask);

        var consumer = new BookCreatedConsumer(logger.Object, bookService.Object);

        var message = new Mock<IBookCreatedMessage>();
        message.SetupGet(x => x.BookId).Returns(bookId);
        message.SetupGet(x => x.Isbn).Returns(isbn);
        message.SetupGet(x => x.UserId).Returns(userId);

        var consumeContext = new Mock<ConsumeContext<IBookCreatedMessage>>();
        consumeContext.SetupGet(x => x.Message).Returns(message.Object);

        await consumer.Consume(consumeContext.Object);

        bookService.Verify(
            x => x.EnrichBookDataAsync(bookId, isbn, userId),
            Times.Once);
    }
}

