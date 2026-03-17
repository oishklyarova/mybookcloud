using Microsoft.EntityFrameworkCore;
using MyBookCloud.Business.Books;
using MyBookCloud.Business.Users;

namespace MyBookCloud.Persistence
{
    public class MyBookCloudDbContext : DbContext
    {
        public MyBookCloudDbContext(DbContextOptions<MyBookCloudDbContext> options) : base(options) { }

        public MyBookCloudDbContext() { }

        public DbSet<BookEntity> Books => Set<BookEntity>();
        public DbSet<UserEntity> Users => Set<UserEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookEntity>().Property(e => e.Id).IsRequired();
            modelBuilder.Entity<BookEntity>().Property(e => e.Title).IsRequired();
            modelBuilder.Entity<BookEntity>().Property(e => e.Author).IsRequired();
            modelBuilder.Entity<BookEntity>().Property(e => e.Status).IsRequired();
            modelBuilder.Entity<BookEntity>().Property(e => e.PersonalRating);
            modelBuilder.Entity<BookEntity>().Property(e => e.CoverThumbnailUrl);
            modelBuilder.Entity<BookEntity>().Property(e => e.PageCount);
            modelBuilder.Entity<BookEntity>().Property(e => e.CreatedById).IsRequired();
            modelBuilder.Entity<BookEntity>()
                        .HasOne<UserEntity>()
                        .WithMany()
                        .HasForeignKey(e => e.CreatedById)
                        .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<UserEntity>().Property(e => e.Id).IsRequired();
            modelBuilder.Entity<UserEntity>().Property(e => e.Email).IsRequired();
            modelBuilder.Entity<UserEntity>().Property(e => e.PasswordHash).IsRequired();
        }
    }
}
