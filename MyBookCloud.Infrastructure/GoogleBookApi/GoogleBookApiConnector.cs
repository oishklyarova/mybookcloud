using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyBookCloud.Application.Connectors;
using MyBookCloud.Application.Dto;

namespace MyBookCloud.Infrastructure.GoogleBookApi
{
    public class GoogleBookApiConnector : IGoogleBookApiConnector
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GoogleBookApiConnector> _logger;
        private readonly string? _apiKey;

        public GoogleBookApiConnector(IConfiguration configuration,
                                      ILogger<GoogleBookApiConnector> logger)
        {
            _logger = logger;
            _apiKey = configuration["GoogleBooks:ApiKey"];

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.googleapis.com"),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        public async Task<GoogleBookVolumeInfoDto?> GetVolumeInfoAsync(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogWarning("GoogleBooks:ApiKey is not configured. Skipping Google Books lookup.");
                return null;
            }

            try
            {
                var requestUri = $"/books/v1/volumes?q=isbn:{Uri.EscapeDataString(isbn)}&key={_apiKey}";
                var response = await _httpClient.GetAsync(requestUri);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Google Books API returned non-success status code {StatusCode} for ISBN {Isbn}.",
                        response.StatusCode, isbn);
                    return null;
                }

                await using var stream = await response.Content.ReadAsStreamAsync();

                var apiResponse = await JsonSerializer.DeserializeAsync<GoogleBooksApiResponseDto>(
                    stream,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (apiResponse == null || apiResponse.TotalItems <= 0 || apiResponse.Items == null || apiResponse.Items.Count == 0)
                {
                    _logger.LogInformation("No Google Books volume found for ISBN {Isbn}.", isbn);
                    return null;
                }

                var volumeInfo = apiResponse.Items[0].VolumeInfo;
                if (volumeInfo == null)
                {
                    _logger.LogInformation("Google Books volumeInfo is missing for ISBN {Isbn}.", isbn);
                    return null;
                }

                return volumeInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling Google Books API for ISBN {Isbn}.", isbn);
                return null;
            }
        }
    }
}

