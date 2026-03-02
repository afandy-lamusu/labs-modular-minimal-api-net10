using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularStore.Api.Data;
using ModularStore.Api.Modules.Orders.Domain;

namespace ModularStore.Api.Modules.Orders.Infrastructure;

public class OrdersDbContext : BaseDbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options, "orders") { }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.TotalPrice).HasPrecision(18, 2);
        builder.HasIndex(o => o.CreatedAt);
        builder.HasIndex(o => o.ProductId);
    }
}
