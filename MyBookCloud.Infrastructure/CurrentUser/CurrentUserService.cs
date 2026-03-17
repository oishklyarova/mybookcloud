using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MyBookCloud.Application.Services;

namespace MyBookCloud.Infrastructure.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(idClaim, out var id))
                {
                    return id;
                }

                return null;
            }
        }
    }
}

