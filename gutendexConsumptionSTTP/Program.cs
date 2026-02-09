using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string url = "https://gutendex.com/books/";
        HttpClient client = new();

        HashSet<string> authors = [];

        string? nextUrl = url;
        int pages = 0;

        while (nextUrl != null && pages < 5)
        {
            string json = await client.GetStringAsync(nextUrl);

            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            // Get books
            JsonElement results = root.GetProperty("results");

            foreach (JsonElement book in results.EnumerateArray())
            {
                if (!book.TryGetProperty("authors", out JsonElement authorsList))
                    continue;

                // Authors isolation
                foreach (JsonElement author in authorsList.EnumerateArray())
                {
                    if (!author.TryGetProperty("name", out JsonElement nameElement))
                        continue;

                    string? name = nameElement.GetString();

                    if (!string.IsNullOrWhiteSpace(name))
                        authors.Add(name.Trim());
                }
            }

            // Next page
            if (root.TryGetProperty("next", out JsonElement next)
                && next.ValueKind != JsonValueKind.Null)
                nextUrl = next.GetString();
            else
                nextUrl = null;

            pages++;
        }

        // Write file
        string filePath = "authors.txt";
        await File.WriteAllLinesAsync(filePath, authors);

        Console.WriteLine($"Completed! {authors.Count} authors were written in {filePath}");
    }
}
