using Microsoft.EntityFrameworkCore;
using ModularStore.Api.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string not found.");

builder.Services.AddDbContext<MigrationsDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions => 
    {
        npgsqlOptions.EnableRetryOnFailure(3);
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "shared");
    }));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.MapOpenApi();

app.MapGet("/", () => "The Modular Store API is Running!");

app.Run();
