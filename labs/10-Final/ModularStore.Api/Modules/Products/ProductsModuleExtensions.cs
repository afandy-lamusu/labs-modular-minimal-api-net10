using Microsoft.EntityFrameworkCore;
using FluentValidation;
using ModularStore.Api.Modules.Products.Application;
using ModularStore.Api.Modules.Products.Domain;
using ModularStore.Api.Modules.Products.Infrastructure;
using ModularStore.Api.Common.Contracts;

namespace ModularStore.Api.Modules.Products;

public static class ProductsModuleExtensions
{
    public static IServiceCollection AddProductsModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ProductsDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(3);
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "products");
            }).UseSnakeCaseNamingConvention());

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IProductModule, ProductModuleApi>();
        services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

        return services;
    }
}
