using System.Text.Json.Serialization;

namespace MyBookCloud.Core.Api.Dto
{
    public class GoogleBookImageLinksDto
    {
        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; set; }
    }

    public class GoogleBookVolumeInfoDto
    {
        [JsonPropertyName("pageCount")]
        public int? PageCount { get; set; }

        [JsonPropertyName("imageLinks")]
        public GoogleBookImageLinksDto? ImageLinks { get; set; }

        /// <summary>
        /// Convenience property to expose thumbnail URL directly.
        /// </summary>
        [JsonIgnore]
        public string? ThumbnailUrl => ImageLinks?.Thumbnail;
    }

    // Internal DTOs for deserializing Google Books API response
    internal class GoogleBooksApiResponseDto
    {
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        [JsonPropertyName("items")]
        public List<GoogleBookVolumeDto>? Items { get; set; }
    }

    internal class GoogleBookVolumeDto
    {
        [JsonPropertyName("volumeInfo")]
        public GoogleBookVolumeInfoDto? VolumeInfo { get; set; }
    }
}

