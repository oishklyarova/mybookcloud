using System;

namespace MyBookCloud.Application.Services
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
    }
}