using Moq;
using MyBookCloud.Application.Services;
using MyBookCloud.Application.Services.Impl;
using MyBookCloud.Business.Users;

namespace MyBookCloud.Application.Tests;

[TestClass]
public class AuthServiceTests
{
    [TestMethod]
    public async Task LoginAsync_ReturnsNull_WhenUserIsNotFound()
    {
        var email = "user@example.com";
        var password = "any-pass";

        var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        userRepository
            .Setup(x => x.FindAsync(email))
            .ReturnsAsync((UserEntity?)null);

        var jwtService = new Mock<IJwtTokenService>(MockBehavior.Strict);

        var sut = new AuthService(userRepository.Object, jwtService.Object);

        var token = await sut.LoginAsync(email, password);

        Assert.IsNull(token);
        jwtService.Verify(x => x.CreateToken(It.IsAny<UserEntity>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_ReturnsNull_WhenPasswordDoesNotMatch()
    {
        var email = "user@example.com";
        var password = "wrong-pass";
        var correctPassword = "correct-pass";

        var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        userRepository
            .Setup(x => x.FindAsync(email))
            .ReturnsAsync(new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword)
            });

        var jwtService = new Mock<IJwtTokenService>(MockBehavior.Strict);

        var sut = new AuthService(userRepository.Object, jwtService.Object);

        var token = await sut.LoginAsync(email, password);

        Assert.IsNull(token);
        jwtService.Verify(x => x.CreateToken(It.IsAny<UserEntity>()), Times.Never);
    }

    [TestMethod]
    public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        var email = "user@example.com";
        var password = "correct-pass";

        var userRepository = new Mock<IUserRepository>(MockBehavior.Strict);
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };
        userRepository
            .Setup(x => x.FindAsync(email))
            .ReturnsAsync(user);

        var expectedToken = "jwt-token";
        var jwtService = new Mock<IJwtTokenService>(MockBehavior.Strict);
        jwtService
            .Setup(x => x.CreateToken(user))
            .Returns(expectedToken);

        var sut = new AuthService(userRepository.Object, jwtService.Object);

        var token = await sut.LoginAsync(email, password);

        Assert.AreEqual(expectedToken, token);
        jwtService.Verify(x => x.CreateToken(user), Times.Once);
    }
}

