using ModularStore.Api.Modules.Orders.Application;

namespace ModularStore.Api.Modules.Orders.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/orders");

        group.MapPost("/", async (PlaceOrderRequest request, IOrderService service, CancellationToken ct) =>
        {
            var orderId = await service.PlaceOrderAsync(request.ProductId, request.Quantity, ct);
            return Results.Created($"/api/orders/{orderId}", new { OrderId = orderId });
        });
    }
}

public record PlaceOrderRequest(Guid ProductId, int Quantity);
