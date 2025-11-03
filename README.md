# RepoTasks

Three independent .NET assignments. Each task sits in its own folder with code and setup steps.

## Structure
```
repotasks/
- task1/  C# console helpers
- task2/  Library analytics API
- task3/  HR API + Angular client
```

## Quick Start
- Task 1: `cd task1 && dotnet run`
- Task 2: `dotnet build task2/task2.sln` then `dotnet run --project task2/src/Library.Api --urls http://localhost:5000`
- Task 3 API: `dotnet build task3/Task3.Hr.sln` then `dotnet run --project task3/Hr.Api --urls http://localhost:5042`
- Task 3 client (CMD, not powershell) : `cd task3/hr-client && npm install && npm run start:proxy`

See individual task READMEs for details.
