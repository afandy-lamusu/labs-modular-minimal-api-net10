namespace ModularStore.Api.Modules.Orders.Domain;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
