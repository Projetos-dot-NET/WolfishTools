using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Wolfish.Rita;

public class AppDbContext : DbContext
{
    public DbSet<DocumentRecord> DocumentRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Convert the float[] array into a JSON string to be stored in SQLite
        modelBuilder.Entity<DocumentRecord>()
            .Property(e => e.Embedding)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null ? null : JsonSerializer.Deserialize<float[]>(v, (JsonSerializerOptions?)null)
            );
    }
}
