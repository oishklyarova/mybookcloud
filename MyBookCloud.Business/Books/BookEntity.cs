using MyBookCloud.Business.SeedWork;

namespace MyBookCloud.Data.Entities
{
    public class BookEntity : BaseEntity
    {
        public required string Title { get; set; }

        public required string Author { get; set; }

        public string? Isbn { get; set; }

        public double? AverageRating { get; set; }

        public string? Note { get; set; }
    }
}
