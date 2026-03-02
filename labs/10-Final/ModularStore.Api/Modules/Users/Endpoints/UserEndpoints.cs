using ModularStore.Api.Modules.Users.Application;

namespace ModularStore.Api.Modules.Users.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        // POST /api/users/register
        group.MapPost("/register", async (
            RegisterRequest request,
            UserService service) =>
        {
            var (success, errors) = await service.RegisterAsync(request);

            return success
                ? Results.Created("/api/users/me", new { message = "Registered." })
                : Results.ValidationProblem(
                    errors.ToDictionary(e => "error", e => new[] { e }));
        })
        .AllowAnonymous();

        // POST /api/users/login
        group.MapPost("/login", async (
            LoginRequest request,
            UserService service) =>
        {
            var response = await service.LoginAsync(request);
            return response is null
                ? Results.Unauthorized()
                : Results.Ok(response);
        })
        .RequireRateLimiting("LoginPolicy")
        .AllowAnonymous();

        // GET /api/users/me — requires a valid JWT
        group.MapGet("/me", (HttpContext ctx) =>
        {
            var userId = ctx.User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var email  = ctx.User.FindFirst(
                System.Security.Claims.ClaimTypes.Email)?.Value;
            var role   = ctx.User.FindFirst(
                System.Security.Claims.ClaimTypes.Role)?.Value;

            return Results.Ok(new { userId, email, role });
        })
        .RequireAuthorization();

        // POST /api/users/assign-role — Admin only
        group.MapPost("/assign-role", async (
            AssignRoleRequest request,
            UserService service) =>
        {
            var success = await service.AssignRoleAsync(
                request.Email, request.Role);
            return success ? Results.Ok() : Results.NotFound();
        })
        .RequireAuthorization("AdminOnly");
    }
}

public record AssignRoleRequest(string Email, string Role);
