<!-- Release v1.2.0: Revised architecture documentation for enhanced clarity -->

# SchoolPortal API Architecture

## Overview

SchoolPortal API follows a clean, modular architecture based on .NET 9 Minimal APIs. This document outlines the architectural patterns, component organization, and design principles.

## Architecture

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   API Endpoints │────►│   Repositories  │────►│     Database    │
└─────────────────┘     └─────────────────┘     └─────────────────┘
        │                       │                       │
        ▼                       ▼                       ▼
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│    Validation   │     │     Models      │     │  Stored Procs   │
└─────────────────┘     └─────────────────┘     └─────────────────┘
```

## Key Components

### Endpoints

Endpoints are implemented using the `IEndpoint` interface:

```csharp
public interface IEndpoint
{
    void MapServices(IServiceCollection services);
    void MapEndpoints(WebApplication app);
}
```

Each endpoint class (Profiles, Institutions, Location) implements this interface to:

- Register domain-specific services
- Map route handlers to HTTP verbs and paths
- Define request/response contracts

### Repositories

Repository classes provide data access using Dapper:

- **ProfileRepository**: Accesses educational profiles-related data
- **InstitutionRepository**: Retrieves institution data including educational profiles and exam results
- **LocationRepository**: Queries location-based data

Repositories inject an `IDbConnectionFactory` for database connectivity.

### Validation

The API uses FluentValidation for request validation:

- Custom validators ensure request data integrity (e.g., `SchoolYearValidator`, `GradeValidator`)
- Validation results are transformed by middleware into standard error responses

### Middleware

Custom middleware components include:

- **ErrorHandlingMiddleware**: Centralizes error handling and returns structured responses
- **RequestLoggingMiddleware**: Logs incoming requests uniformly

### Health Checks

Health checks monitor:

- Database connectivity
- API health and self-diagnostics
- Integration with telemetry systems

## Project Structure

```
SchoolPortal/
├── SchoolPortal.Api/                // API implementation
│   ├── Endpoints/                   // API endpoint definitions
│   ├── Extensions/                  // Extension methods
│   ├── Infrastructure/              // Cross-cutting concerns
│   │   └── HealthChecks/            // Health check implementations
│   ├── Middlewares/                 // Custom middleware
│   ├── Models/                      // Data models and DTOs
│   ├── Properties/                  // Contains AssemblyInfo.cs and launchSettings.json
│   ├── Repositories/                // Data access layer
│   ├── Validation/                  // Request validators
│   └── wwwroot/                     // Static assets
|
├── SchoolPortal.Database.Deploy/    // Database deployment scripts
│   ├── AlwaysRun/                   // Contains functions, stored procedures and views scripts
│   └── Scripts/                     // Contains all the scripts for initialize and create the tables
|
├── SchoolPortal.IntegrationTests/   // Integration tests
│   └── Fixtures/                    // Test fixtures
|
├── SchoolPortal.UnitTests/          // Unit tests
│   |── Endpoints/                   // Endpoint unit tests
|   └── Validation/                  // Validation unit tests
|
└── docs/                            // Documentation
```

## Request Processing Flow

1. Client sends an HTTP request to an endpoint.
2. `RequestLoggingMiddleware` logs the incoming request.
3. The endpoint processes the request.
4. Validators check the request data (if needed).
5. Repositories access the database using stored procedures.
6. The response is serialized and returned to the client.
7. Any exceptions are caught by `ErrorHandlingMiddleware`.

## Error Handling Strategy

- `ErrorHandlingMiddleware` catches all unhandled exceptions.
- Exceptions are converted to standardized `ErrorResponse` objects.
- The middleware assigns HTTP status codes based on exception types:
  - ValidationException → 400 Bad Request
  - KeyNotFoundException → 404 Not Found
  - All other exceptions → 500 Internal Server Error

## Telemetry and Monitoring

The API integrates OpenTelemetry for:

1. Distributed tracing of incoming requests.
2. Custom metrics collection and logging.
3. Publishing health check statuses.

## Testing Strategy

- **Unit Tests**: Individual components are tested in isolation.
- **Integration Tests**: End-to-end tests run against a test database.
- **Test Fixtures**: Shared contexts and test data setup.
- **Test Data Seeding**: Helpers populate databases for testing.

## Deployment Architecture

The deployment strategy includes:

1. **Development Environment**

   - Hosts the latest feature branches for testing and QA.
   - URL: https://eduapi-dev.azurewebsites.net

2. **Production Environment**
   - Hosts stable, production-ready releases.
   - URL: https://eduapi.azurewebsites.net

Deployment includes:

- CI/CD pipelines via Azure DevOps.
- Automated testing before deployment.
- Database schema migration and versioning.
- Infrastructure-as-Code for consistent environment management.
