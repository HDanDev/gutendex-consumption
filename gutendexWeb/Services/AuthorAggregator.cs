using System.Collections.Generic;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public class AuthorAggregator(GutendexClient client, IOptions<GutendexOptions> options, ILogger<AuthorAggregator> logger)
    {
        private readonly GutendexClient _client = client;
        private readonly string _baseUrl = options.Value.BaseUrl;
        private readonly ILogger<AuthorAggregator> _logger = logger;

        public async Task<IEnumerable<string>> GetAuthorsAsync(int pages, CancellationToken cancellationToken)
        {
            string? nextUrl = _baseUrl;
            var authors = new HashSet<string>();
            int currentPage = 0;
            _logger.LogInformation("Aggregating {Pages} pages", pages);
            while (nextUrl != null && currentPage < pages)
            {
                var page = await _client.GetPageAsync(nextUrl, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                // Defensive: stop if API response is not as expected
                if (page.Results == null || page.Results.Count == 0)
                {
                    _logger.LogWarning("No results returned from Gutendex.");
                    break;
                }

                foreach (var book in page.Results)
                {
                    if (book.Authors == null)
                        continue;

                    foreach (var author in book.Authors)
                    {
                        if (string.IsNullOrWhiteSpace(author.Name))
                            continue;

                        authors.Add(author.Name.Trim());

                        //// Basic sanity limit, exclude suspiciously long string
                        // var normalizedName = author.Name.Trim();

                        // if (normalizedName.Length <= 200)
                        //     authors.Add(normalizedName);
                    }
                }

                nextUrl = page.Next;
                currentPage++;
            }

            _logger.LogInformation(
                "Aggregation complete. Found {AuthorCount} unique authors.",
                authors.Count);
            return authors;
        }
    }
}
