using ModularStore.Api.Modules.Products.Domain;
using ModularStore.Api.Modules.Products.Application;
using ModularStore.Api.Common.Contracts;

namespace ModularStore.Api.Modules.Products.Infrastructure;

public class ProductModuleApi : IProductModule
{
    private readonly IProductRepository _repository;
    private readonly IProductService _service;

    public ProductModuleApi(IProductRepository repository, IProductService service)
    {
        _repository = repository;
        _service = service;
    }

    public async Task<ProductContractResponse?> GetProductDetailsAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(productId, ct);
        if (product == null) return null;

        return new ProductContractResponse(product.Id, product.Name, product.Price);
    }

    public async Task<bool> DeductStockAsync(Guid productId, int quantity, CancellationToken ct = default)
    {
        return await _service.DeductStockAsync(productId, quantity, ct);
    }
}
