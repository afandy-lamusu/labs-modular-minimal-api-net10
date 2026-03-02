using ModularStore.Api.Modules.Products.Domain;

namespace ModularStore.Api.Modules.Products.Application;

public record ProductResponse(Guid Id, string Name, string Description, decimal Price, int Stock);
public record CreateProductRequest(string Name, string Description, decimal Price, int Stock);

public class ProductService
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

    public async Task<IEnumerable<ProductResponse>> GetProductsAsync(CancellationToken ct)
    {
        var products = await _repository.GetAllAsync(ct);
        return products.Select(p => new ProductResponse(p.Id, p.Name, p.Description, p.Price, p.Stock));
    }
}
