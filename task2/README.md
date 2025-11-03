# Task 2 - Library Analytics API

Read-only ASP.NET Core Web API that answers common library questions (most borrowed titles, availability, co-borrowed books, read rates, top borrowers, grouped history). The solution uses Entity Framework Core with a clear split between API controllers, application services, and infrastructure.

---

## Project Layout
```
task2/
- src/
  - Library.Api/             ASP.NET Core Web API (controllers, Program.cs, configuration)
  - Library.Application/     DTOs and query services
  - Library.Infrastructure/  EF Core DbContext, entities, migrations, seed data
- Library.Tests/             xUnit test suite (in-memory provider)
```

---

## Prerequisites
- .NET SDK 8.0 or later
- SQL Server LocalDB (default) or SQLite (development profile already configured)

Development configuration (`appsettings.Development.json`) uses SQLite, creates `App_Data/library-dev.db`, ensures the schema exists, and loads deterministic sample data. Production configuration targets SQL Server LocalDB with migrations enabled and seeding disabled.

---

## Run the API
```bash
dotnet build task2/task2.sln
ASPNETCORE_ENVIRONMENT=Development dotnet run --project task2/src/Library.Api --urls http://localhost:5000
```

Useful endpoints:
- `GET /` -> text message `"Library API is running..."`.
- `GET /health` -> `{"status":"Healthy","database":true}` when the database is reachable.
- `GET /swagger` -> interactive OpenAPI UI in Development.

---

## Feature Coverage

| Capability | Endpoint | Notes |
|------------|----------|-------|
| Most borrowed books | `GET /api/books/most-borrowed?take=5` | Ranked list with borrow counts |
| Book availability | `GET /api/books/{bookId}/availability` | Totals copies, borrowed copies, remaining availability |
| Related (co-borrowed) titles | `GET /api/books/{bookId}/also-borrowed` | Distinct borrowers and loan counts for related titles |
| Read rate | `GET /api/books/{bookId}/read-rate` | Average pages per day for completed loans |
| Top borrowers | `GET /api/users/top-borrowers?from=2024-01-01&to=2024-12-31&take=3` | Time-boxed ranking |
| Borrow history | `GET /api/users/{userId}/history?groupBy=Month` | Grouped by Day, Week, or Month (validates groupBy) |
| Health | `GET /health` | Confirms app and database status |

All queries execute on the database server; no large in-memory aggregations are performed.

---

## Configuration Flags
`Program.cs` consumes the following keys (with development defaults):

| Setting | Purpose | Development default |
|---------|---------|---------------------|
| `DatabaseProvider` | `SqlServer` or `Sqlite` | `Sqlite` |
| `RunMigrationsOnStartup` | Run migrations or ensure schema | `true` |
| `SeedOnStartup` | Execute `LibrarySeed.EnsureSeedDataAsync` | `true` |
| `ConnectionStrings:DefaultConnection` | Provider-specific connection string | SQLite file |

Override via environment variables (for example `DatabaseProvider=SqlServer`) or additional JSON files for production deployments.

---

## Tests
```bash
dotnet test task2/task2.sln
```

Tests use the EF Core InMemory provider with seeded data to verify each analytic scenario.

---

## Troubleshooting
- Ensure the selected provider matches the connection string. SQLite paths are relative to the content root; bootstrap code creates directories automatically.
- When switching to SQL Server, set `DatabaseProvider=SqlServer`, provide a valid connection string, and keep `RunMigrationsOnStartup=true` so migrations execute on startup.
