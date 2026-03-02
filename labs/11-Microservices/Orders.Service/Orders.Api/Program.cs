using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Text;
using ModularStore.Api.Data;
using ModularStore.Api.Modules.Orders;
using ModularStore.Api.Modules.Orders.Endpoints;
using ModularStore.Api.Modules.Orders.Infrastructure;
using ModularStore.Api.Common.Contracts;
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
var jwtKey = builder.Configuration["JWT_SECRET"] ?? "DEVELOPMENT_ONLY_KEY_32_CHARS_LONG!!!";
var key = Encoding.ASCII.GetBytes(jwtKey);

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

// 5. Register Orders Module
builder.Services.AddOrdersModule(connectionString);

// Register HttpClient for Products contract
builder.Services.AddHttpClient<IProductModule, ProductsHttpClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ProductsUrl"] ?? "http://localhost:8080");
    client.Timeout = TimeSpan.FromSeconds(5);
});

// 6. Infrastructure
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks().AddNpgSql(connectionString);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Orders.Service"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());

var app = builder.Build();

// 6.5. Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var providers = scope.ServiceProvider;
    var db = providers.GetRequiredService<ModularStore.Api.Modules.Orders.Infrastructure.OrdersDbContext>();
    await db.Database.MigrateAsync();
}

// 7. Middleware
app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.MapOpenApi();

// 8. Endpoints
app.MapHealthChecks("/health");
app.MapOrderEndpoints();

app.MapGet("/", () => "Orders Service (Extracted) Ready!");

app.Run();
