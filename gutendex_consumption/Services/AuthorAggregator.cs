using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Services
{
    public class AuthorAggregator(GutendexClient client)
    {
        private readonly GutendexClient _client = client;

        public async Task<HashSet<string>> GetAuthorsAsync(int pages)
        {
            string? nextUrl = "https://gutendex.com/books/";
            var authors = new HashSet<string>();
            int currentPage = 0;

            while (nextUrl != null && currentPage < pages)
            {
                var page = await _client.GetPageAsync(nextUrl);

                // Defensive: stop if API response is not as expected
                if (page.Results == null || page.Results.Count == 0)
                    break;

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

            return authors;
        }
    }
}
