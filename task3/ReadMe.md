# Task 3 - HR Employee Manager

Web app for managing employees. Includes a .NET 9 API (EF Core + SQL Server) and an Angular 20 frontend.

## Run
```bash
dotnet build task3/Task3.Hr.sln
# Requires SQL Server LocalDB (or update `appsettings.Development.json` with another SQL Server connection string if you want to use a different database).
# https://go.microsoft.com/fwlink/p/?linkid=2216019&clcid=0x409&culture=en-us&country=us SQL SERVER 2022v EXPRESS DOWNLOAD LINK
dotnet run --project task3/Hr.Api --urls http://localhost:5042

cd task3/hr-client
npm install
npm run start:proxy
```

Browse `http://localhost:4200`. API listens on `http://localhost:5042`.
