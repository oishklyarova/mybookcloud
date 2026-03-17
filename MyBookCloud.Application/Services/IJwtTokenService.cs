using MyBookCloud.Business.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Application.Services
{
    public interface IJwtTokenService
    {
        string CreateToken(UserEntity user);
    }
}
