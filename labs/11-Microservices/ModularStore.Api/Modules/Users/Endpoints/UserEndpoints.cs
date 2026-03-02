using System.Security.Claims;
using ModularStore.Api.Modules.Users.Application;

namespace ModularStore.Api.Modules.Users.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users");

        group.MapPost("/login", async (LoginRequest request, UserService service, CancellationToken ct) =>
        {
            var response = await service.AuthenticateAsync(request, ct);
            return response == null 
                ? Results.Unauthorized() 
                : Results.Ok(response);
        }).RequireRateLimiting("LoginPolicy");

        group.MapGet("/me", (ClaimsPrincipal user) =>
        {
            return Results.Ok(new
            {
                Id = user.FindFirstValue(ClaimTypes.NameIdentifier),
                Email = user.FindFirstValue(ClaimTypes.Email),
                Role = user.FindFirstValue(ClaimTypes.Role)
            });
        }).RequireAuthorization();
    }
}
