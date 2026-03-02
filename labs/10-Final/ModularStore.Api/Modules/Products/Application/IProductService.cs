namespace ModularStore.Api.Modules.Products.Application;

public interface IProductService
{
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request, CancellationToken ct);
    Task<IEnumerable<ProductResponse>> GetProductsAsync(int page, int pageSize, CancellationToken ct);
    Task<bool> DeductStockAsync(Guid productId, int quantity, CancellationToken ct);
}
