using System.Text.Json.Serialization;

namespace MyBookCloud.Application.Dto
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
}

