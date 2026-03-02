using Microsoft.EntityFrameworkCore;

namespace ModularStore.Api.Data;

public abstract class BaseDbContext : DbContext
{
    protected readonly string Schema;

    protected BaseDbContext(DbContextOptions options, string schema) : base(options)
    {
        Schema = schema;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if (!string.IsNullOrWhiteSpace(Schema))
        {
            modelBuilder.HasDefaultSchema(Schema);
        }
        base.OnModelCreating(modelBuilder);
    }
}
