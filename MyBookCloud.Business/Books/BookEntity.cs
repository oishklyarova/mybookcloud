using MyBookCloud.Business.SeedWork;

namespace MyBookCloud.Business.Books
{
    public class BookEntity : BaseEntity
    {
        public required string Title { get; set; }

        public required string Author { get; set; }

        public string? Isbn { get; set; }

        public double? AverageRating { get; set; }

        public string? Note { get; set; }

        public ReadingStatus Status { get; set; }

        public int? PersonalRating { get; set; }
    }
}
