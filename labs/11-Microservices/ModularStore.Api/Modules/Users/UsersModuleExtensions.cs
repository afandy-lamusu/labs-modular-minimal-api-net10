using Microsoft.EntityFrameworkCore;
using ModularStore.Api.Modules.Users.Application;
using ModularStore.Api.Modules.Users.Infrastructure;

namespace ModularStore.Api.Modules.Users;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(3);
                npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "users");
            }).UseSnakeCaseNamingConvention());

        services.AddScoped<UserService>();

        return services;
    }
}
