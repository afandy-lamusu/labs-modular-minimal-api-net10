namespace ModularStore.Api.Modules.Orders.Application;

public interface IOrderService
{
    Task<Guid> PlaceOrderAsync(Guid productId, int quantity, CancellationToken ct = default);
}
