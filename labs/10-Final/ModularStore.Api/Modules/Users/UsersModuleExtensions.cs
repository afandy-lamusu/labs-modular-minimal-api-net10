using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModularStore.Api.Modules.Users.Application;
using ModularStore.Api.Modules.Users.Infrastructure;

namespace ModularStore.Api.Modules.Users;

public static class UsersModuleExtensions
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        string connectionString)
    {
        // Register the Users DbContext
        services.AddDbContext<UsersDbContext>(opt =>
            opt.UseNpgsql(connectionString)
               .UseSnakeCaseNamingConvention());

        // Register Identity with our custom DbContext
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            // Password policy
            options.Password.RequiredLength         = 8;
            options.Password.RequireDigit           = true;
            options.Password.RequireUppercase        = false;
            options.Password.RequireNonAlphanumeric  = false;

            // Lockout after 5 failed attempts for 15 minutes
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan  =
                TimeSpan.FromMinutes(15);
            options.Lockout.AllowedForNewUsers      = true;

            // Require unique emails
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<UsersDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<UserService>();

        return services;
    }
}
