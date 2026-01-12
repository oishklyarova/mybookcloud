using Microsoft.EntityFrameworkCore;
using MyBookCloud.Data.Entities;

namespace MyBookCloud.Persistence
{
    public class MyBookCloudDbContext : DbContext
    {
        public MyBookCloudDbContext(DbContextOptions<MyBookCloudDbContext> options) : base(options) { }

        public MyBookCloudDbContext() { }

        public DbSet<BookEntity> Books => Set<BookEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookEntity>().Property(e => e.Id).IsRequired();
            modelBuilder.Entity<BookEntity>().Property(e => e.Title).IsRequired();
            modelBuilder.Entity<BookEntity>().Property(e => e.Author).IsRequired();

        }
    }
}
