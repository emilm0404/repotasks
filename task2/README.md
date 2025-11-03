# Task 2 - Library Analytics API

Read-only ASP.NET Core API that reports on library activity using Entity Framework Core.

## Capabilities
- Most borrowed books and availability per title
- Top borrowers for a date range and per-user borrow history
- Co-borrowed titles and read-rate (pages per day) calculations

## Run
```bash
dotnet build task2/task2.sln
ASPNETCORE_ENVIRONMENT=Development dotnet run --project task2/src/Library.Api --urls http://localhost:5000
```

Swagger: `/swagger`  
Health check: `/health`
