using Microsoft.EntityFrameworkCore;
using ModularStore.Api.Data;
using ModularStore.Api.Modules.Products;
using ModularStore.Api.Modules.Products.Endpoints;
using ModularStore.Api.Modules.Products.Infrastructure;
using ModularStore.Api.Modules.Orders.Infrastructure;
using ModularStore.Api.Modules.Orders.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<MigrationsDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => 
    {
        npgsqlOptions.EnableRetryOnFailure(3);
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "shared");
    }));

builder.Services.AddProductsModule(connectionString);
builder.Services.AddOrdersModule(connectionString);

// Register the cross-module contract
builder.Services.AddScoped<IProductModule, ProductModuleApi>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapOpenApi();
app.MapProductEndpoints();
app.MapOrderEndpoints();

app.MapGet("/", () => "Modular Store API with Product and Orders Modules Ready!");

app.Run();
