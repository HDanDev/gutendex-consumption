using System;
using System.Threading.Tasks;
using Services;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            using var codetimer = new CodeTimer("Whole Program");

            var client = new GutendexClient();
            var aggregator = new AuthorAggregator(client);
            var authors = await aggregator.GetAuthorsAsync(5);

            var writer = new FileWriter();
            await writer.WriteAsync("authors.txt", authors);

            Console.WriteLine($"Completed! {authors.Count} authors were written in authors.txt");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error : {ex.Message}");
        }
    }
}
