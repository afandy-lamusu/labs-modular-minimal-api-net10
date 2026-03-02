using ModularStore.Api.Modules.Products.Domain;

namespace ModularStore.Api.Modules.Products.Infrastructure;

public class ProductModuleApi : IProductModule
{
    private readonly IProductRepository _repository;

    public ProductModuleApi(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductContractResponse?> GetProductDetailsAsync(
        Guid productId, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(productId, ct);
        if (product == null) return null;

        return new ProductContractResponse(
            product.Id, product.Name, product.Price);
    }
}
