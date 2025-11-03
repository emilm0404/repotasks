using Hr.Application.Abstractions;
using Hr.Application.Services;
using Hr.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<HrDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowClient4200", p => p.WithOrigins("http://localhost:4200")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod());
});

var runMigrations = builder.Configuration.GetValue("RunMigrationsOnStartup", builder.Environment.IsDevelopment());
var seedOnStartup = builder.Configuration.GetValue("SeedOnStartup", builder.Environment.IsDevelopment());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowClient4200");
app.MapControllers();

if (runMigrations || seedOnStartup)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<HrDbContext>();

    if (runMigrations)
    {
        await db.Database.MigrateAsync();
    }

    if (seedOnStartup)
    {
        await Seed.EnsureSeedAsync(db);
    }
}

app.Run();
