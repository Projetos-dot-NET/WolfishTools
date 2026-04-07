using System.ComponentModel.DataAnnotations;

namespace Wolfish.Rita;

public class DocumentRecord
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string TextContent { get; set; } = string.Empty;

    public int RetrievalCount { get; set; } = 0;

    // We will map this to a JSON or string representation in SQLite using EF Core Value Converters
    public float[]? Embedding { get; set; }
}
