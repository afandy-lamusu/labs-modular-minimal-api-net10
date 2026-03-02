using ModularStore.Api.Modules.Products.Application;
using FluentValidation;

namespace ModularStore.Api.Modules.Products.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
                       .WithTags("Products");

        group.MapPost("/", async (
            CreateProductRequest request, 
            ProductService service, 
            IValidator<CreateProductRequest> validator, 
            CancellationToken ct) =>
        {
            var validationResult = await validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var product = await service.CreateProductAsync(request, ct);
            return Results.Created($"/api/products/{product.Id}", product);
        });

        group.MapGet("/", async (ProductService service, CancellationToken ct) =>
        {
            return Results.Ok(await service.GetProductsAsync(ct));
        });
    }
}
