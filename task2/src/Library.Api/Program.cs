using System.Data.Common;
using System.IO;
using Library.Application.Interfaces;
using Library.Application.Services;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var databaseProvider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "SqlServer";
var runMigrations = builder.Configuration.GetValue("RunMigrationsOnStartup", builder.Environment.IsDevelopment());
var seedOnStartup = builder.Configuration.GetValue("SeedOnStartup", builder.Environment.IsDevelopment());

builder.Services.AddDbContext<LibraryDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    if (string.Equals(databaseProvider, "Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        var connectionBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };
        if (connectionBuilder.TryGetValue("Data Source", out var dataSourceObj) && dataSourceObj is string dataSourceValue)
        {
            if (!Path.IsPathRooted(dataSourceValue))
            {
                var fullPath = Path.Combine(builder.Environment.ContentRootPath, dataSourceValue);
                var directory = Path.GetDirectoryName(fullPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                connectionBuilder["Data Source"] = fullPath;
            }
        }
        else
        {
            var fullPath = Path.Combine(builder.Environment.ContentRootPath, connectionString);
            var directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            connectionBuilder["Data Source"] = fullPath;
        }

        options.UseSqlite(connectionBuilder.ConnectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

if (runMigrations || seedOnStartup)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    var isSqlite = string.Equals(databaseProvider, "Sqlite", StringComparison.OrdinalIgnoreCase);

    if (runMigrations)
    {
        if (isSqlite)
        {
            await db.Database.EnsureCreatedAsync();
        }
        else
        {
            await db.Database.MigrateAsync();
        }
    }

    if (seedOnStartup)
    {
        await LibrarySeed.EnsureSeedDataAsync(db);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Library API is running...");
app.MapGet("/health", async (LibraryDbContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();
    return Results.Ok(new { status = "Healthy", database = canConnect });
});

app.MapControllers();

app.Run();
