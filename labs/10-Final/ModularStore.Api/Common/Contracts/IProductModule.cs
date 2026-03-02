namespace ModularStore.Api.Common.Contracts;

public interface IProductModule
{
    Task<ProductContractResponse?> GetProductDetailsAsync(Guid productId, CancellationToken ct = default);
    Task<bool> DeductStockAsync(Guid productId, int quantity, CancellationToken ct = default);
}

public record ProductContractResponse(Guid Id, string Name, decimal Price);
