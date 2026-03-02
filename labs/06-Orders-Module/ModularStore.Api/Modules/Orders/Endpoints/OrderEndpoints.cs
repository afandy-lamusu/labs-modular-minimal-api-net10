using ModularStore.Api.Modules.Orders.Application;

namespace ModularStore.Api.Modules.Orders.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders")
                       .WithTags("Orders");

        group.MapPost("/", async (
            PlaceOrderRequest request,
            OrderService service,
            CancellationToken ct) =>
        {
            var orderId = await service.PlaceOrderAsync(
                request.ProductId, request.Quantity, ct);
            return Results.Created($"/api/orders/{orderId}",
                new { Id = orderId });
        });

        group.MapGet("/", async (
            OrderService service,
            CancellationToken ct) =>
        {
            var orders = await service.GetAllAsync(ct);
            return Results.Ok(orders);
        });
    }
}

public record PlaceOrderRequest(Guid ProductId, int Quantity);
