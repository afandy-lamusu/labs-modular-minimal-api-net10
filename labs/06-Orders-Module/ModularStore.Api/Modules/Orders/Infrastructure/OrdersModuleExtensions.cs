using Microsoft.EntityFrameworkCore;
using ModularStore.Api.Modules.Orders.Application;
using ModularStore.Api.Modules.Orders.Domain;
using ModularStore.Api.Modules.Orders.Infrastructure;

namespace ModularStore.Api.Modules.Orders.Infrastructure;

public static class OrdersModuleExtensions
{
    public static IServiceCollection AddOrdersModule(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<OrdersDbContext>(opt =>
            opt.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "orders");
            }).UseSnakeCaseNamingConvention());

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<OrderService>();

        return services;
    }
}
