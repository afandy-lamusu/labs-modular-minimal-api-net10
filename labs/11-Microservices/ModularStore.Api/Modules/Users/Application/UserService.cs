using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModularStore.Api.Modules.Users.Infrastructure;

namespace ModularStore.Api.Modules.Users.Application;

public record LoginRequest(string Email, string Password);
public record TokenResponse(string Token);

public class UserService
{
    private readonly UsersDbContext _context;
    private readonly IConfiguration _configuration;

    public UserService(UsersDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<TokenResponse?> AuthenticateAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
        
        // ⚠ In a real app, use BCrypt or similar to verify PasswordHash
        // For this ebook example, we simplify login logic:
        if (user == null || request.Password != "password123") return null;

        var token = GenerateJwt(user);
        return new TokenResponse(token);
    }

    private string GenerateJwt(Domain.User user)
    {
        var jwtKey = _configuration["JWT_SECRET"] ?? "DEVELOPMENT_ONLY_KEY_32_CHARS_LONG!!!";
        var key = Encoding.ASCII.GetBytes(jwtKey);

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
