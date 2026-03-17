using MyBookCloud.Business.Books;
using MyBookCloud.Business.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Business.Users
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity?> FindAsync(string email);
    }
}
