using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ModularStore.Api.Modules.Users.Application;

public class UserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;

    public UserService(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration config)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
    }

    // ── Register ──────────────────────────────────────────
    public async Task<(bool Success, IEnumerable<string> Errors)>
        RegisterAsync(RegisterRequest request)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email    = request.Email
        };

        // Identity handles password hashing — we just pass the plain text
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return (false, result.Errors.Select(e => e.Description));

        // Assign default role on registration
        await EnsureRoleExistsAsync("Customer");
        await _userManager.AddToRoleAsync(user, "Customer");

        return (true, []);
    }

    // ── Login ─────────────────────────────────────────────
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return null;

        // Identity checks the password hash — never compare plain text
        var isValid = await _userManager.CheckPasswordAsync(
            user, request.Password);
        if (!isValid) return null;

        // Check if account is locked out
        if (await _userManager.IsLockedOutAsync(user)) return null;

        var roles   = await _userManager.GetRolesAsync(user);
        var role    = roles.FirstOrDefault() ?? "Customer";
        var expiry  = DateTime.UtcNow.AddMinutes(
            _config.GetValue<int>("Jwt:TokenExpiryMinutes", 60));
        var token   = GenerateToken(user, roles, expiry);

        return new LoginResponse(token, user.Email!, role, expiry);
    }

    // ── Assign Role ───────────────────────────────────────
    public async Task<bool> AssignRoleAsync(string email, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        await EnsureRoleExistsAsync(role);
        var result = await _userManager.AddToRoleAsync(user, role);
        return result.Succeeded;
    }

    // ── JWT Generation ────────────────────────────────────
    private string GenerateToken(
        IdentityUser user,
        IList<string> roles,
        DateTime expiry)
    {
        var jwtKey = _config["JWT_SECRET"]
                     ?? "DEVELOPMENT_ONLY_KEY_MINIMUM_32_CHARS!!";

        var key         = new SymmetricSecurityKey(
                              Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(
                              key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub,   user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
        };

        // Add each role as a separate claim
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            expires:            expiry,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task EnsureRoleExistsAsync(string role)
    {
        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole(role));
    }
}
