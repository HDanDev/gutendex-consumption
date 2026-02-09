using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Models
{
    public class GutendexResponse
    {
        [JsonPropertyName("results")]
        public List<Book>? Results { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }
    }

    public class Book
    {
        [JsonPropertyName("authors")]
        public List<Author>? Authors { get; set; }
    }

    public class Author
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
