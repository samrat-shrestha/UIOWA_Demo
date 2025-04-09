# UIOWA Receipt Reimbursement System

A full-stack web application for university employees to submit and manage receipts for reimbursement.

## Project Overview

This application provides a clean, user-friendly interface for employees to submit receipts for reimbursement. The system captures purchase details (date, amount, description) and allows uploading receipt files (PDF, PNG, JPEG). Users can view their submitted receipts and download the receipt files.

## Technology Stack

### Backend
- **Framework**: .NET 9.0 (C#)
- **Architecture**: Clean Architecture with CQRS pattern
- **Database**: SQLite (for simplicity, can be easily replaced with SQL Server, PostgreSQL, etc.)
- **Libraries**: MediatR, FluentValidation, Entity Framework Core
- **Authentication**: API Key for simplicity (would use JWT or OAuth in production)

### Frontend
- **Framework**: Angular 19
- **UI Components**: Angular Material
- **Authentication**: Okta Auth

### Why This Stack?
- **.NET**: Provides excellent performance, robust API design capabilities, and strong typing
- **Angular**: Offers component-based architecture, TypeScript for better type safety, and great tooling
- **Clean Architecture**: Ensures separation of concerns, testability, and maintainability
- **CQRS with MediatR**: Simplifies handling business logic and promotes single responsibility
- **SQLite**: Lightweight database requiring no setup, perfect for demonstration purposes
- **Okta**: Enterprise-grade authentication with minimal configuration

## Project Structure

- `/src` - Source code
  - `/Domain` - Domain entities, value objects, and business logic
  - `/Application` - Application services, commands, and queries
  - `/Infrastructure` - Infrastructure concerns (database, file storage, etc.)
  - `/UIOWA_WebApi` - Web API project
  - `/UIOWA_ClientApp` - Client application (Angular)

- `/tests` - Test projects
  - `/Domain.Tests` - Tests for the Domain layer
  - `/Application.Tests` - Tests for the Application layer
  - `/Infrastructure.Tests` - Tests for the Infrastructure layer
  - `/UIOWA_WebApi.Tests` - Tests for the Web API layer

## Setup and Running the Application

### Prerequisites
- .NET 9.0 SDK
- Node.js (v18+) and npm
- Git

### Running the Backend API

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/UIOWA.git
   cd UIOWA
   ```

2. Build and run the API:
   ```bash
   dotnet build
   cd src/UIOWA_WebApi
   dotnet run
   ```

   The API will be available at:
   - http://localhost:5291
   - https://localhost:7038

   Swagger documentation is available at:
   - http://localhost:5291/swagger
   - https://localhost:7038/swagger

### Running the Frontend Application

1. Navigate to the Angular client app directory:
   ```bash
   cd src/UIOWA_ClientApp/angular-client-app
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the Angular development server:
   ```bash
   npm start
   ```

   The frontend will be available at:
   - http://localhost:4200

   **Note**: Make sure the backend API is running before using the frontend application.

### Application Usage

1. **Login**: Use the Okta authentication to log in
2. **View Receipts**: See a list of all submitted receipts
3. **Submit Receipt**: Fill out the form with purchase details and upload a receipt file
4. **View Details**: Click on any receipt to view its details and download the receipt file

## Database Management

### Migrations

To create a new migration:
```bash
dotnet ef migrations add MigrationName --project src/Infrastructure --startup-project src/UIOWA_WebApi
```

To apply migrations:
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/UIOWA_WebApi
```

## Assumptions and Design Decisions

1. **SQLite Database**: Used for simplicity, but the application is designed to work with any Entity Framework-compatible database
2. **File Storage**: Receipts are stored in the local filesystem for demonstration purposes; in production, this would be replaced with cloud storage
3. **Authentication**: Implemented Okta authentication to demonstrate enterprise-level security
4. **Form Validation**: Both client-side and server-side validation ensure data integrity
5. **Clean Architecture**: Used to maintain separation of concerns and testability

## Testing

To run the backend tests:
```bash
dotnet test
```
