# Task 3 - HR Employee Manager

Web app for managing employees. Includes a .NET 9 API (EF Core + SQL Server) and an Angular 20 frontend.

## Run
```bash
dotnet build task3/Task3.Hr.sln
ASPNETCORE_ENVIRONMENT=Development dotnet run --project task3/Hr.Api --urls http://localhost:5042

cd task3/hr-client
npm install
npm run start:proxy
```

Browse `http://localhost:4200`. API listens on `http://localhost:5042`.
