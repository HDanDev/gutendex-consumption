using System;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models;

namespace Services
{
    // Client responsible for communicating with the Gutendex public API. Handles HTTP calls, response validation, and JSON deserialization.
    public class GutendexClient(HttpClient httpClient, ILogger<GutendexClient> logger)
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<GutendexClient> _logger = logger;

        // Retrieves a single page of books from the Gutendex API.
        public async Task<GutendexResponse> GetPageAsync(string url, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching Gutendex page: {Url}", url);
                using var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Gutendex returned {StatusCode}", response.StatusCode);
                    throw new HttpRequestException(
                        $"API returned {(int)response.StatusCode} ({response.ReasonPhrase})");
                }
                if (response.Content.Headers.ContentType?.MediaType != "application/json")
                    throw new Exception("Unexpected content type received from API.");

                var data = await response.Content.ReadFromJsonAsync<GutendexResponse>(cancellationToken: cancellationToken);


                if (data == null)
                {
                    _logger.LogError("Deserialized Gutendex response was null.");
                    throw new Exception("Empty API response.");
                }

                return data;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error calling Gutendex API", ex);
            }
        }
    }
}
