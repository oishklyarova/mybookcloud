using MyBookCloud.Application.Dto;
using System.Text.Json.Serialization;

namespace MyBookCloud.Infrastructure.GoogleBookApi
{
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
