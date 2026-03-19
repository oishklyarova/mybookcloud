using Microsoft.Extensions.Configuration;
using MyBookCloud.Business.Users;
using MyBookCloud.Infrastructure.Jwt;
using System.IdentityModel.Tokens.Jwt;

namespace MyBookCloud.Infrastructure.Tests;

[TestClass]
public class JwtTokenServiceTests
{
    [TestMethod]
    public void CreateToken_ContainsExpectedClaims()
    {
        var userId = Guid.NewGuid();
        var email = "user@example.com";
        var expDays = 7;

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["Jwt:Key"] = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                ["Jwt:ExpireDays"] = expDays.ToString()
            })
            .Build();

        var sut = new JwtTokenService(config);

        var user = new UserEntity
        {
            Id = userId,
            Email = email,
            PasswordHash = "hash"
        };

        var token = sut.CreateToken(user);
        Assert.IsFalse(string.IsNullOrWhiteSpace(token));

        var handler = new JwtSecurityTokenHandler();
        handler.InboundClaimTypeMap.Clear(); // keep original claim types from the token
        var jwt = handler.ReadJwtToken(token);

        Assert.IsNotNull(jwt);
        Assert.IsTrue(jwt.Claims.Any(c => c.Value == userId.ToString()), "Expected a claim containing userId.");
        Assert.IsTrue(jwt.Claims.Any(c => c.Value == email), "Expected a claim containing email.");

        // JwtTokenService uses DateTime.Now, but the JWT handler can interpret expiration in UTC.
        // So compare in "days until expiration" with a generous tolerance.
        Assert.IsTrue(jwt.ValidTo > DateTime.UtcNow, "Token expiration should be in the future.");
        var daysUntilExpiration = (jwt.ValidTo - DateTime.UtcNow).TotalDays;
        Assert.IsTrue(Math.Abs(daysUntilExpiration - expDays) < 0.1, "Token expiration is outside expected tolerance.");
    }
}

