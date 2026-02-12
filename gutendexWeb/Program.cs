using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<GutendexClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddScoped<AuthorAggregator>();
builder.Services.AddScoped<FileWriter>();
builder.Services.Configure<GutendexOptions>(
    builder.Configuration.GetSection("Gutendex"));

var app = builder.Build();

app.MapGet("/", () => "Gutendex Author Fetcher");

// URL pattern : /authors?pages=[PageNbr]&sort=[letter]
app.MapGet("/authors", async (int pages, string? sort, AuthorAggregator aggregator, FileWriter FileWriter, CancellationToken cancellationToken) =>
    {
        using var timer = new CodeTimer("GET /authors");

        if (pages <= 0 || pages > 10)
            return Results.BadRequest("Pages must be between 1 and 10.");

        var authors = await aggregator.GetAuthorsAsync(pages, cancellationToken);

        if (!string.IsNullOrWhiteSpace(sort))
        {
            if (sort.Length != 1 || !char.IsLetter(sort[0]))
                return Results.BadRequest("sort must be a single letter.");

            var letter = sort[0];

            authors = authors
                .Where(a => a.Length > 0 &&
                    char.ToUpperInvariant(a[0]) ==
                    char.ToUpperInvariant(letter));
        }

        var content = string.Join(Environment.NewLine, authors);
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);

        var fileName = $"authors_{DateTime.UtcNow:yyyyMMddHHmmss}.txt";

        return Results.File(bytes, "text/plain", fileName);
    });

app.Run();
