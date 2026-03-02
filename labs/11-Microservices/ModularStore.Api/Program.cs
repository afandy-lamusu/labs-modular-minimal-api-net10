using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Text;
using ModularStore.Api.Data;
using ModularStore.Api.Modules.Products;
using ModularStore.Api.Modules.Users;
using ModularStore.Api.Modules.Products.Endpoints;
using ModularStore.Api.Modules.Users.Endpoints;
using ModularStore.Api.Common.Middleware;
using Serilog;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// 1. Logging
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();

// 2. Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found.");

// 4. Security & Performance
var jwtKey = builder.Configuration["JWT_SECRET"];
if (string.IsNullOrEmpty(jwtKey))
{
    if (builder.Environment.IsProduction())
        throw new InvalidOperationException("JWT_SECRET environment variable is missing!");
    
    jwtKey = "DEVELOPMENT_ONLY_KEY_32_CHARS_LONG!!!";
}
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5;
        opt.QueueLimit = 0;
    });
});

// 5. Register Modules
builder.Services.AddProductsModule(connectionString);
builder.Services.AddUsersModule(connectionString);

// 6. Infrastructure
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks().AddNpgSql(connectionString);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("ModularStore.Api"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());

var app = builder.Build();

// 6.5. Apply Migrations (Optional: For Development/Staging)
using (var scope = app.Services.CreateScope())
{
    var providers = scope.ServiceProvider;
    var contexts = new[] { 
        typeof(ModularStore.Api.Modules.Products.Infrastructure.ProductsDbContext),
        typeof(ModularStore.Api.Modules.Users.Infrastructure.UsersDbContext)
    };

    foreach (var contextType in contexts)
    {
        var db = (DbContext)providers.GetRequiredService(contextType);
        await db.Database.MigrateAsync();
    }
}

// 7. Middleware
app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapOpenApi();

// 8. Endpoints
app.MapHealthChecks("/health");
app.MapProductEndpoints();
app.MapUserEndpoints();

app.MapGet("/", () => "Modular Store Monolith (Extracted Orders) Ready!");

app.Run();
