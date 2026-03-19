using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyBookCloud.Application.Services;
using MyBookCloud.Business.Users;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace MyBookCloud.Infrastructure.Jwt
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly string _jwtKey;
        private readonly int _expireDays;

        public JwtTokenService(IConfiguration config)
        {
            _jwtKey = config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key is missing in configuration.");

            _expireDays = config.GetValue<int>("Jwt:ExpireDays");

            if (_expireDays <= 0) _expireDays = 7;
        }

        public string CreateToken(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(_expireDays),
                SigningCredentials = creds,
                Issuer = null,
                Audience = null
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}