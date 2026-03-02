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

public class MigrationsDbContext : DbContext
{
    public MigrationsDbContext(DbContextOptions<MigrationsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("shared");
        modelBuilder.Entity<Modules.Products.Domain.Product>()
                    .ToTable("Products", "products");
        base.OnModelCreating(modelBuilder);
    }
}
