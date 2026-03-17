using Microsoft.EntityFrameworkCore;
using MyBookCloud.Business.Books;
using MyBookCloud.Business.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Persistence.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(MyBookCloudDbContext context) : base(context)
        {
        }

        public async Task<UserEntity?> FindAsync(string email)
        {
            var user = await context.Users
                .Where(b => b.Email.ToLower().Trim() == email.ToLower().Trim())
                .SingleOrDefaultAsync();

            return user;
        }
    }
}
