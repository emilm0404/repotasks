# HR Employee Manager (Task 3)

Web application for HR to list, add, edit, and delete employees. The solution contains a .NET 9 API with EF Core plus SQL Server and an Angular 20 client.

## Prerequisites
- .NET SDK 9.0 or later
- SQL Server (LocalDB works for local development)
- Node.js 20 or later
- npm 10 or later

## Initial Setup
1. Navigate to the task folder: `cd task3`
2. Restore tools (needed for migrations): `dotnet tool restore`
3. Apply the initial database migration:
   `dotnet tool run dotnet-ef database update --project Hr.Infrastructure --startup-project Hr.Api`
4. Development startup now applies migrations and seed data automatically (see configuration section below). For production, run migrations explicitly and keep `SeedOnStartup` disabled.

## Configuration

`Hr.Api` reads the following settings (defaults shown for Development):

| Setting | Description | Dev default | Production suggestion |
|---------|-------------|-------------|------------------------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string | LocalDB (`HrDevDb`) | Point to production SQL instance |
| `RunMigrationsOnStartup` | Apply EF Core migrations during boot | `true` | `false` (run migrations via pipeline) |
| `SeedOnStartup` | Execute demo seed (`Seed.EnsureSeedAsync`) | `true` | `false` |

Override these values in environment-specific JSON files or environment variables (for example `RunMigrationsOnStartup=false`).

## Running the API
```
dotnet run --project Hr.Api
```
The API listens on `http://localhost:5042` (see `launchSettings.json`). Swagger UI is enabled in Development.

## Running the Angular Client
```
cd hr-client
npm install
npm run start:proxy
```
The proxy configuration forwards `/api` calls to the API, so the Angular app works without CORS issues.

## Testing
- Backend tests: `dotnet test`
- Frontend unit tests: `npm test`

## Notable Features
- Employee CRUD endpoints with optimistic concurrency (`rowversion` column)
- Server-side paging/sorting plus tokenized search (matches first name, last name, or combined full name)
- Angular client with reactive forms, validation aligned with backend rules, and friendly conflict/error messaging
- Seed helper populates 200 sample employees on a fresh database

## Troubleshooting
- If migrations fail because the database exists, drop the `HrDb` database and re-run the migration command.
- The API requires a reachable SQL Server instance defined in `Hr.Api/appsettings.json` (`DefaultConnection`). Update the connection string if you do not use LocalDB.
