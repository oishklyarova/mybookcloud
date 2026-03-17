using MyBookCloud.Business.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Business.Users
{
    public class UserEntity : BaseEntity
    {
        public required string Email { get; set; }

        public required string PasswordHash { get; set; }
    }
}
