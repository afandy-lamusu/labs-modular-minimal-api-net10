using Microsoft.EntityFrameworkCore;
using ModularStore.Api.Data;
using ModularStore.Api.Modules.Orders.Domain;

namespace ModularStore.Api.Modules.Orders.Infrastructure;

public class OrdersDbContext : BaseDbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options, "orders") // Isolated schema for Orders
    {
    }

    public DbSet<Order> Orders => Set<Order>();
}
