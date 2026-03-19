using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyBookCloud.Application.Connectors;
using MyBookCloud.Application.Dto;
using System.Net.Http.Json;

namespace MyBookCloud.Infrastructure.GoogleBookApi
{
    public class GoogleBookApiConnector : IGoogleBookApiConnector
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleBookApiConnector> _logger;
        private readonly string? _apiKey;

        public GoogleBookApiConnector(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<GoogleBookApiConnector> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiKey = configuration["GoogleBooks:ApiKey"];
        }

        public async Task<GoogleBookVolumeInfoDto?> GetVolumeInfoAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn) || string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogWarning("ISBN or GoogleBooks:ApiKey is missing.");
                return null;
            }

            try
            {
                var requestUri = $"books/v1/volumes?q=isbn:{Uri.EscapeDataString(isbn)}&key={_apiKey}";

                var response = await _httpClient.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Google Books API error: {StatusCode} for ISBN {Isbn}",
                        response.StatusCode, isbn);
                    return null;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<GoogleBooksApiResponseDto>(
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return apiResponse?.Items?.FirstOrDefault()?.VolumeInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Google Books API call for ISBN {Isbn}", isbn);
                return null;
            }
        }
    }
}