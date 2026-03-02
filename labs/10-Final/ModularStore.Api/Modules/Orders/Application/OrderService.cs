using ModularStore.Api.Modules.Orders.Domain;
using ModularStore.Api.Common.Contracts;

namespace ModularStore.Api.Modules.Orders.Application;

public class OrderService : IOrderService
{
    private readonly IProductModule _productModule;
    private readonly IOrderRepository _orderRepository;

    public OrderService(IProductModule productModule, IOrderRepository orderRepository)
    {
        _productModule = productModule;
        _orderRepository = orderRepository;
    }

    public async Task<Guid> PlaceOrderAsync(Guid productId, int quantity, CancellationToken ct = default)
    {
        var product = await _productModule.GetProductDetailsAsync(productId, ct);
        if (product == null) throw new KeyNotFoundException($"Product with ID {productId} not found.");

        // DEDUCT STOCK
        var success = await _productModule.DeductStockAsync(productId, quantity, ct);
        if (!success) throw new InvalidOperationException("Could not deduct stock. Maybe insufficient quantity?");

        var order = new Order(productId, quantity, product.Price);
        await _orderRepository.AddAsync(order, ct);
        await _orderRepository.SaveChangesAsync(ct);
        return order.Id;
    }
}
