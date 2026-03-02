namespace ModularStore.Api.Modules.Orders.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Order() { }

    public Order(Guid productId, int quantity, decimal unitPrice)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        TotalPrice = unitPrice * quantity;
        CreatedAt = DateTime.UtcNow;
    }
}
