using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using MyBookCloud.Infrastructure.CurrentUser;

namespace MyBookCloud.Infrastructure.Tests;

[TestClass]
public class CurrentUserServiceTests
{
    [TestMethod]
    public void UserId_ReturnsGuid_WhenNameIdentifierClaimIsValid()
    {
        var userId = Guid.NewGuid();

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "test"))
        };

        var accessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
        accessor.SetupGet(x => x.HttpContext).Returns(httpContext);

        var sut = new CurrentUserService(accessor.Object);

        Assert.AreEqual(userId, sut.UserId);
    }

    [TestMethod]
    public void UserId_ReturnsNull_WhenNameIdentifierClaimIsMissing()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "test"))
        };

        var accessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
        accessor.SetupGet(x => x.HttpContext).Returns(httpContext);

        var sut = new CurrentUserService(accessor.Object);

        Assert.IsNull(sut.UserId);
    }

    [TestMethod]
    public void UserId_ReturnsNull_WhenNameIdentifierClaimIsNotAGuid()
    {
        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "not-a-guid")
            }, "test"))
        };

        var accessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
        accessor.SetupGet(x => x.HttpContext).Returns(httpContext);

        var sut = new CurrentUserService(accessor.Object);

        Assert.IsNull(sut.UserId);
    }
}

