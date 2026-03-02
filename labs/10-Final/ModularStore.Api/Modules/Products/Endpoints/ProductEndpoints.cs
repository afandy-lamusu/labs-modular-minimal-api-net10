using ModularStore.Api.Modules.Products.Application;
using ModularStore.Api.Common.Filters;

namespace ModularStore.Api.Modules.Products.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products");

        group.MapPost("/", async (CreateProductRequest request, IProductService service, CancellationToken ct) =>
        {
            var product = await service.CreateProductAsync(request, ct);
            return Results.Created($"/api/products/{product.Id}", product);
        })
        .AddEndpointFilter<ValidationFilter<CreateProductRequest>>()
        .RequireAuthorization("AdminOnly");

        group.MapGet("/", async (IProductService service, CancellationToken ct, int page = 1, int pageSize = 20) =>
        {
            return Results.Ok(await service.GetProductsAsync(page, pageSize, ct));
        });
    }
}
