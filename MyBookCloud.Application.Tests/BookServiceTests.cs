using AutoMapper;
using MassTransit;
using Moq;
using MyBookCloud.Application.AutoMapperProfiles;
using MyBookCloud.Application.Connectors;
using MyBookCloud.Application.Dto;
using MyBookCloud.Application.Services;
using MyBookCloud.Application.Services.Impl;
using MyBookCloud.Business.Books;
using MyBookCloud.Business.SeedWork;
using MyBookCloud.Common.Messages;
using MyBookCloud.Persistence;

namespace MyBookCloud.Application.Tests;

[TestClass]
public class BookServiceTests
{
    private static IMapper CreateMapper()
    {
        var cfg = new MapperConfiguration(x => x.AddProfile(new BookProfile()));
        return cfg.CreateMapper();
    }

    [TestMethod]
    public async Task AddBookAsync_Throws_WhenUserIsNotAuthenticated()
    {
        var mapper = CreateMapper();

        var bookRepository = new Mock<IBookRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork<MyBookCloudDbContext>>(MockBehavior.Strict);
        var publish = new Mock<IPublishEndpoint>(MockBehavior.Strict);
        var googleConnector = new Mock<IGoogleBookApiConnector>(MockBehavior.Strict);
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUser.SetupGet(x => x.UserId).Returns((Guid?)null);

        var sut = new BookService(
            mapper,
            bookRepository.Object,
            unitOfWork.Object,
            publish.Object,
            googleConnector.Object,
            currentUser.Object);

        var input = new BookData
        {
            Id = Guid.NewGuid(),
            Title = "Title",
            Author = "Author",
            Isbn = "978-111",
            Status = ReadingStatus.Reading
        };

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => sut.AddBookAsync(input));

        bookRepository.Verify(x => x.Add(It.IsAny<BookEntity>()), Times.Never);
        publish.Verify(
            x => x.Publish<IBookCreatedMessage>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task AddBookAsync_SavesChanges_AndPublishesCreatedMessage()
    {
        var mapper = CreateMapper();

        var currentUserId = Guid.NewGuid();
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUser.SetupGet(x => x.UserId).Returns(currentUserId);

        BookEntity? addedEntity = null;
        var bookRepository = new Mock<IBookRepository>(MockBehavior.Strict);
        bookRepository
            .Setup(x => x.Add(It.IsAny<BookEntity>()))
            .Callback<BookEntity>(e => addedEntity = e);

        var unitOfWork = new Mock<IUnitOfWork<MyBookCloudDbContext>>(MockBehavior.Strict);
        var expectedBookId = Guid.NewGuid();
        unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Callback((bool _, CancellationToken _) =>
            {
                if (addedEntity != null)
                    addedEntity.Id = expectedBookId; // simulate EF assigning key on SaveChanges
            })
            .ReturnsAsync(1);

        object? publishedValues = null;
        var publish = new Mock<IPublishEndpoint>(MockBehavior.Strict);
        publish
            .Setup(x => x.Publish<IBookCreatedMessage>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((values, _) => publishedValues = values)
            .Returns(Task.CompletedTask);

        var googleConnector = new Mock<IGoogleBookApiConnector>(MockBehavior.Strict);

        var sut = new BookService(
            mapper,
            bookRepository.Object,
            unitOfWork.Object,
            publish.Object,
            googleConnector.Object,
            currentUser.Object);

        var inputIsbn = "978-222";
        var input = new BookData
        {
            Id = Guid.NewGuid(),
            Title = "Title",
            Author = "Author",
            Isbn = inputIsbn,
            Status = ReadingStatus.Reading
        };

        var result = await sut.AddBookAsync(input);

        Assert.IsNotNull(addedEntity);
        Assert.AreEqual(expectedBookId, result.Id);
        Assert.AreEqual(input.Title, result.Title);
        Assert.AreEqual(input.Author, result.Author);
        Assert.AreEqual(inputIsbn, result.Isbn);

        publish.Verify(
            x => x.Publish<IBookCreatedMessage>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.IsNotNull(publishedValues);
        var bookIdFromMessage = (Guid)publishedValues!.GetType().GetProperty("BookId")!.GetValue(publishedValues)!;
        var isbnFromMessage = (string?)publishedValues.GetType().GetProperty("Isbn")!.GetValue(publishedValues);
        var userIdFromMessage = (Guid)publishedValues.GetType().GetProperty("UserId")!.GetValue(publishedValues)!;

        Assert.AreEqual(expectedBookId, bookIdFromMessage);
        Assert.AreEqual(inputIsbn, isbnFromMessage);
        Assert.AreEqual(currentUserId, userIdFromMessage);
    }

    [TestMethod]
    public async Task UpdateBookAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        var mapper = CreateMapper();

        var bookRepository = new Mock<IBookRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork<MyBookCloudDbContext>>(MockBehavior.Strict);
        var publish = new Mock<IPublishEndpoint>(MockBehavior.Strict);
        var googleConnector = new Mock<IGoogleBookApiConnector>(MockBehavior.Strict);
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUser.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var bookId = Guid.NewGuid();
        bookRepository.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync((BookEntity?)null);

        var sut = new BookService(mapper, bookRepository.Object, unitOfWork.Object, publish.Object, googleConnector.Object, currentUser.Object);

        var result = await sut.UpdateBookAsync(bookId, new BookData
        {
            Id = Guid.NewGuid(),
            Title = "New",
            Author = "New",
            Status = ReadingStatus.Reading
        });

        Assert.IsNull(result);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        bookRepository.Verify(x => x.Update(It.IsAny<BookEntity>()), Times.Never);
    }

    [TestMethod]
    public async Task UpdateBookAsync_UpdatesAndSaves_WhenBookExists()
    {
        var mapper = CreateMapper();

        var bookId = Guid.NewGuid();
        var existing = new BookEntity
        {
            Id = bookId,
            Title = "Old title",
            Author = "Old author",
            Isbn = "978-old",
            Status = ReadingStatus.Reading,
            CreatedById = Guid.NewGuid()
        };

        var bookRepository = new Mock<IBookRepository>(MockBehavior.Strict);
        bookRepository.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync(existing);
        bookRepository.Setup(x => x.Update(existing));

        var unitOfWork = new Mock<IUnitOfWork<MyBookCloudDbContext>>(MockBehavior.Strict);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var publish = new Mock<IPublishEndpoint>(MockBehavior.Strict);
        var googleConnector = new Mock<IGoogleBookApiConnector>(MockBehavior.Strict);

        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUser.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var sut = new BookService(mapper, bookRepository.Object, unitOfWork.Object, publish.Object, googleConnector.Object, currentUser.Object);

        var input = new BookData
        {
            Id = Guid.NewGuid(),
            Title = "New title",
            Author = "New author",
            Isbn = "978-new",
            Status = ReadingStatus.Finished
        };

        var result = await sut.UpdateBookAsync(bookId, input);

        Assert.IsNotNull(result);
        Assert.AreEqual(bookId, result!.Id);
        Assert.AreEqual(input.Title, result.Title);
        Assert.AreEqual(input.Author, result.Author);
        Assert.AreEqual(input.Isbn, result.Isbn);
        Assert.AreEqual(input.Status, result.Status);

        bookRepository.Verify(x => x.Update(existing), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task DeleteBookAsync_ReturnsFalse_WhenBookDoesNotExist()
    {
        var mapper = CreateMapper();

        var bookRepository = new Mock<IBookRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork<MyBookCloudDbContext>>(MockBehavior.Strict);
        var publish = new Mock<IPublishEndpoint>(MockBehavior.Strict);
        var googleConnector = new Mock<IGoogleBookApiConnector>(MockBehavior.Strict);
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUser.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var bookId = Guid.NewGuid();
        bookRepository.Setup(x => x.GetByIdAsync(bookId)).ReturnsAsync((BookEntity?)null);

        var sut = new BookService(mapper, bookRepository.Object, unitOfWork.Object, publish.Object, googleConnector.Object, currentUser.Object);

        var result = await sut.DeleteBookAsync(bookId);

        Assert.IsFalse(result);
        bookRepository.Verify(x => x.Delete(It.IsAny<BookEntity>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task EnrichBookDataAsync_ReturnsEarly_WhenIsbnIsBlank()
    {
        var mapper = CreateMapper();

        var bookRepository = new Mock<IBookRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork<MyBookCloudDbContext>>(MockBehavior.Strict);
        var publish = new Mock<IPublishEndpoint>(MockBehavior.Strict);
        var googleConnector = new Mock<IGoogleBookApiConnector>(MockBehavior.Strict);
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUser.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var sut = new BookService(mapper, bookRepository.Object, unitOfWork.Object, publish.Object, googleConnector.Object, currentUser.Object);

        await sut.EnrichBookDataAsync(Guid.NewGuid(), "  ", Guid.NewGuid());

        googleConnector.Verify(x => x.GetVolumeInfoAsync(It.IsAny<string>()), Times.Never);
        bookRepository.Verify(x => x.FindAsync(It.IsAny<Guid>()), Times.Never);
        publish.Verify(
            x => x.Publish<IBookEnrichedMessage>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task EnrichBookDataAsync_UpdatesEntity_AndPublishesEnrichedMessage()
    {
        var mapper = CreateMapper();

        var bookId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var isbn = "978-333";

        var volumeInfo = new GoogleBookVolumeInfoDto
        {
            PageCount = 123,
            ImageLinks = new GoogleBookImageLinksDto { Thumbnail = "http://thumb" }
        };

        var book = new BookEntity
        {
            Id = bookId,
            Title = "Book",
            Author = "Author",
            Status = ReadingStatus.Reading,
            CreatedById = userId
        };

        var googleConnector = new Mock<IGoogleBookApiConnector>(MockBehavior.Strict);
        googleConnector.Setup(x => x.GetVolumeInfoAsync(isbn)).ReturnsAsync(volumeInfo);

        var bookRepository = new Mock<IBookRepository>(MockBehavior.Strict);
        bookRepository.Setup(x => x.FindAsync(bookId)).ReturnsAsync(book);

        var unitOfWork = new Mock<IUnitOfWork<MyBookCloudDbContext>>(MockBehavior.Strict);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(1);

        object? publishedValues = null;
        var publish = new Mock<IPublishEndpoint>(MockBehavior.Strict);
        publish
            .Setup(x => x.Publish<IBookEnrichedMessage>(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((values, _) => publishedValues = values)
            .Returns(Task.CompletedTask);

        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUser.SetupGet(x => x.UserId).Returns(Guid.NewGuid());

        var sut = new BookService(mapper, bookRepository.Object, unitOfWork.Object, publish.Object, googleConnector.Object, currentUser.Object);

        await sut.EnrichBookDataAsync(bookId, isbn, userId);

        Assert.AreEqual("http://thumb", book.CoverThumbnailUrl);
        Assert.AreEqual(123, book.PageCount);

        publish.Verify(
            x => x.Publish<IBookEnrichedMessage>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.IsNotNull(publishedValues);
        Assert.AreEqual(bookId, (Guid)publishedValues!.GetType().GetProperty("BookId")!.GetValue(publishedValues)!);
        Assert.AreEqual("http://thumb", (string?)publishedValues.GetType().GetProperty("CoverThumbnailUrl")!.GetValue(publishedValues));
        Assert.AreEqual(123, (int?)publishedValues.GetType().GetProperty("PageCount")!.GetValue(publishedValues));
        Assert.AreEqual(userId, (Guid)publishedValues.GetType().GetProperty("UserId")!.GetValue(publishedValues)!);
    }
}

