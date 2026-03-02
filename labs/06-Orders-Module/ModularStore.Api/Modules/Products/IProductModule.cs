namespace ModularStore.Api.Modules.Products;

public interface IProductModule
{
    Task<ProductContractResponse?> GetProductDetailsAsync(
        Guid productId,
        CancellationToken ct = default);
}

public record ProductContractResponse(Guid Id, string Name, decimal Price);
