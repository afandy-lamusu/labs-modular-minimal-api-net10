namespace ModularStore.Api.Modules.Users.Application;

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);

public record LoginResponse(
    string Token,
    string Email,
    string Role,
    DateTime ExpiresAt);
