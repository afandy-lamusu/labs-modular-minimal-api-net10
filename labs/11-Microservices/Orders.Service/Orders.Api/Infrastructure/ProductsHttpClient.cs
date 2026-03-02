using System.Net.Http.Json;
using ModularStore.Api.Common.Contracts;

namespace ModularStore.Api.Modules.Orders.Infrastructure;

public class ProductsHttpClient : IProductModule
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductsHttpClient> _logger;

    public ProductsHttpClient(
        HttpClient httpClient,
        ILogger<ProductsHttpClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ProductContractResponse?> GetProductDetailsAsync(
        Guid productId, CancellationToken ct)
    {
        try
        {
            return await _httpClient
                .GetFromJsonAsync<ProductContractResponse>(
                    $"api/products/{productId}", ct);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "Failed to reach Products service for product {Id}",
                productId);
            return null;
        }
    }

    public async Task<bool> DeductStockAsync(Guid productId, int quantity, CancellationToken ct)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(
                $"api/products/{productId}/deduct-stock", 
                new { Quantity = quantity }, ct);
            
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "Failed to deduct stock from Products service for product {Id}",
                productId);
            return false;
        }
    }
}
