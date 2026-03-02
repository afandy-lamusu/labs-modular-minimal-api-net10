using ModularStore.Api.Modules.Orders.Domain;

namespace ModularStore.Api.Modules.Orders.Infrastructure;

public class OrderRepository : IOrderRepository
{
    private readonly OrdersDbContext _context;

    public OrderRepository(OrdersDbContext context) => _context = context;

    public async Task AddAsync(Order order, CancellationToken ct) 
        => await _context.Orders.AddAsync(order, ct);

    public async Task SaveChangesAsync(CancellationToken ct) 
        => await _context.SaveChangesAsync(ct);
}
