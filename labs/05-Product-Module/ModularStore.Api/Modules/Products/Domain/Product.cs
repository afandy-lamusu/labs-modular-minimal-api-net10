namespace ModularStore.Api.Modules.Products.Domain;

public class Product
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int Stock { get; private set; }

    private Product() { }

    public Product(string name, string description, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty");
        if (price < 0) throw new ArgumentException("Price cannot be negative");

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0) throw new ArgumentException("Price cannot be negative");
        if (newPrice > Price * 1.5m) throw new ArgumentException("Price hike cannot exceed 50% in a single update.");
        Price = newPrice;
    }
}
