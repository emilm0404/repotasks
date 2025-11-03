# RepoTasks

This repository packages three independent assignments that highlight different areas of the .NET stack: a console app with utility methods, a read-only analytics API, and a full-stack HR system with an Angular front end. Each task lives in its own folder with code, documentation, and tooling.

---

## Repository Layout
```
repotasks/
- task1/  Basic C# console program with utility helpers
- task2/  Library analytics Web API (ASP.NET Core + EF Core)
- task3/  HR employee management API plus Angular client
```

---

## Task Highlights

### Task 1 - Basic Methods
- Demonstrates bitwise checks, string reversal, replication, and odd-number printing.
- Implementation lives in `task1.cs`, compiled by `Task1.csproj`.
- Run with:
  ```bash
  cd task1
  dotnet run
  ```

### Task 2 - Library Analytics API
- ASP.NET Core Web API exposing read-only borrowing insights (most borrowed, availability, co-borrowed, read rates, top borrowers, grouped history).
- Layered solution: controllers in `Library.Api`, query logic in `Library.Application`, EF Core data access in `Library.Infrastructure`.
- Run with:
  ```bash
  dotnet build task2/task2.sln
  ASPNETCORE_ENVIRONMENT=Development dotnet run --project task2/src/Library.Api --urls http://localhost:5000
  ```
  Swagger is available at `/swagger`; `/health` reports database status.

### Task 3 - HR Employee App
- .NET 9 Web API delivering optimistic concurrency, paging, sorting, and tokenized search that matches first name, last name, or combined full name.
- Angular 20 client proxies API calls, uses reactive forms, and handles conflicts gracefully.
- Run with:
  ```bash
  dotnet build task3/Task3.Hr.sln
  ASPNETCORE_ENVIRONMENT=Development dotnet run --project task3/Hr.Api --urls http://localhost:5042

  cd task3/hr-client
  npm install
  npm run start:proxy
  ```
  Visit `http://localhost:4200` for the client while the API listens on port `5042`.

---

## Tooling and Prerequisites
- .NET SDK 9.0 (task2 targets net8.0; task3 uses net9.0 and EF Core 9 tooling)
- SQL Server LocalDB (default) or SQLite for task2; SQL Server LocalDB for task3
- Node.js 20 and npm 10 for the Angular client (task3)
- Optional REST client (for example Postman or VS Code REST Client) for manual API checks

Restore the shared EF Core tool from the repo root when needed:
```bash
dotnet tool restore
```

---

## Additional Notes
- Each task folder ships with its own README that dives into setup, architecture, and troubleshooting.
- Tasks are isolated: databases, configs, and package dependencies do not overlap.
- Use the repo as a playground for experimentation or evaluation without affecting the other tasks.
