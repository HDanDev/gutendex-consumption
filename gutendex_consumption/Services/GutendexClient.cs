using System;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Models;

namespace Services
{
    // Client responsible for communicating with the Gutendex public API. Handles HTTP calls, response validation, and JSON deserialization.
    public class GutendexClient
    {
        private readonly HttpClient _httpClient;

        public GutendexClient()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        // Retrieves a single page of books from the Gutendex API.
        public async Task<GutendexResponse> GetPageAsync(string url)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException(
                        $"API returned {(int)response.StatusCode} ({response.ReasonPhrase})");

                if (response.Content.Headers.ContentType?.MediaType != "application/json")
                    throw new Exception("Unexpected content type received from API.");

                var json = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<GutendexResponse>(json);

                return data ?? throw new Exception("Empty API response.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Gutendex API error: {ex.Message}", ex);
            }
        }
    }
}
