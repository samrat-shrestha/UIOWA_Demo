# UIOWA Project

A Clean Architecture-based receipt management system.

## Project Structure

- `/src` - Source code
  - `/Domain` - Domain entities, value objects, and business logic
  - `/Application` - Application services, commands, and queries
  - `/Infrastructure` - Infrastructure concerns (database, file storage, etc.)
  - `/UIOWA_WebApi` - Web API project
  - `/UIOWA_ClientApp` - Client application

- `/tests` - Test projects
  - `/Domain.Tests` - Tests for the Domain layer
  - `/Application.Tests` - Tests for the Application layer
  - `/Infrastructure.Tests` - Tests for the Infrastructure layer
  - `/UIOWA_WebApi.Tests` - Tests for the Web API layer

## Getting Started

### Prerequisites
- .NET 9.0 SDK

### Running the API
```bash
cd src/UIOWA_WebApi
dotnet run
```

The API will be available at:
- http://localhost:5291
- https://localhost:7038

Swagger documentation is available at:
- http://localhost:5291/swagger
- https://localhost:7038/swagger # Database Migrations

To create a new migration:
```bash
dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/UIOWA_WebApi
```

To apply migrations:
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/UIOWA_WebApi
```
