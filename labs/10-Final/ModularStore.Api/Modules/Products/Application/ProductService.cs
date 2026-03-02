using ModularStore.Api.Modules.Products.Domain;

namespace ModularStore.Api.Modules.Products.Application;

public record ProductResponse(Guid Id, string Name, string Description, decimal Price, int Stock);
public record CreateProductRequest(string Name, string Description, decimal Price, int Stock);

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository) => _repository = repository;

    public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request, CancellationToken ct)
    {
        var product = new Product(request.Name, request.Description, request.Price, request.Stock);
        await _repository.AddAsync(product, ct);
        await _repository.SaveChangesAsync(ct);
        return new ProductResponse(product.Id, product.Name, product.Description, product.Price, product.Stock);
    }

    public async Task<IEnumerable<ProductResponse>> GetProductsAsync(int page, int pageSize, CancellationToken ct)
    {
        var products = await _repository.GetAllAsync(page, pageSize, ct);
        return products.Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Stock));
    }

    public async Task<bool> DeductStockAsync(Guid productId, int quantity, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(productId, ct);
        if (product == null) return false;

        try
        {
            product.DeductStock(quantity);
            await _repository.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
