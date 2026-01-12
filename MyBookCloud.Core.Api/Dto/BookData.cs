namespace MyBookCloud.Core.Api.Dto
{
    public class BookData
    {
        public Guid Id { get; set; }

        public required string Title { get; set; }

        public required string Author { get; set; }

        public string? Isbn { get; set; }

        public double? AverageRating { get; set; }

        public string? Note { get; set; }
    }
}
