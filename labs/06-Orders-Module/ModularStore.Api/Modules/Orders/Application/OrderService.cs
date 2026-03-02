using ModularStore.Api.Modules.Orders.Domain;
using ModularStore.Api.Modules.Products;

namespace ModularStore.Api.Modules.Orders.Application;

public class OrderService
{
    private readonly IProductModule _productModule;
    private readonly IOrderRepository _orderRepository;

    public OrderService(
        IProductModule productModule,
        IOrderRepository orderRepository)
    {
        _productModule = productModule;
        _orderRepository = orderRepository;
    }

    public async Task<Guid> PlaceOrderAsync(
        Guid productId, int quantity, CancellationToken ct)
    {
        // 1. Call the Products module through its contract — not the DB
        var product = await _productModule
            .GetProductDetailsAsync(productId, ct);

        if (product == null)
            throw new KeyNotFoundException(
                $"Product {productId} not found.");

        // 2. Create the domain object — business rules enforced here
        var order = new Order(productId, quantity, product.Price);

        // 3. Persist through the repository
        await _orderRepository.AddAsync(order, ct);
        await _orderRepository.SaveChangesAsync(ct);

        return order.Id;
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken ct)
    {
        return await _orderRepository.GetAllAsync(ct);
    }
}
