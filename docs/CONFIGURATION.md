# SchoolPortal API Configuration Guide

## Overview

This document outlines the configuration options for the SchoolPortal API, including application settings, middleware, services, and component registration patterns.

## Configuration Files

The API uses the following configuration files:

- **appsettings.json**: Primary configuration for all environments
- **appsettings.{Environment}.json**: Environment-specific settings (Development, Test, Production)

## Core Configuration Sections

### Connection Strings

```json
"ConnectionStrings": {
  "DatabaseConnection": "Server=.;Database=SchoolPortal;User=sa;Password=yourStrong(!)Password;Integrated Security=false;Encrypt=false;TrustServerCertificate=false;Connection Timeout=30;Max Pool Size=60;"
}
```

### CORS Configuration

```json
"Cors": {
  "AllowedOrigins": [
    "https://eduinfo.azurewebsites.net",
    "https://eduinfo-dev.azurewebsites.net",
    "http://localhost:3000",
    "https://kandidatstvam.bg"
  ]
}
```

### Health Checks

```json
"HealthChecks": {
  "HealthCheckUrl": "https://localhost:7154/api/v1/health"
}
```

### OpenTelemetry

The API uses OpenTelemetry for distributed tracing and monitoring. Configuration includes:

- Trace exporting to an OpenTelemetry collector
- Metrics collection and exporting
- Logging integration

### Swagger/OpenAPI Configuration

The API provides comprehensive OpenAPI documentation with the following features:

- **Environment-Specific Access**: Only available in Development environment
- **Dynamic Server Detection**: Automatically configures server URLs based on current request protocol (HTTP/HTTPS)
- **Enhanced UI Features**:
  - Request duration display
  - Try-it-out enabled by default
  - Syntax highlighting options

#### Local Development URLs

- HTTP: `http://localhost:5141/swagger`
- HTTPS: `https://localhost:7154/swagger`

#### Configuration Details

```csharp
services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SchoolPortal API",
        Version = "v1",
        Description = "School Portal API for education information management",
        Contact = new OpenApiContact { Name = "SchoolPortal Team" }
    });
    s.EnableAnnotations();
});
```

- **Self-hosted SigNoz Integration:** The telemetry data is exported to a self-hosted SigNoz instance for tracing and monitoring. Access the logs and traces at [SigNoz Dashboard](http://signozdo.eastus.cloudapp.azure.com:3301/logs/logs-explorer?compositeQuery=%257B%2522queryType%2522%253A%2522builder%2522%252C%2522builder%2522%253A%257B%2522queryData%2522%253A%255B%257B%2522dataSource%2522%253A%2522logs%2522%252C%2522queryName%2522%253A%2522A%2522%252C%2522aggregateOperator%2522%253A%2522noop%2522%252C%2522aggregateAttribute%2522%253A%257B%2522id%2522%253A%2522------%2522%252C%2522dataType%2522%253A%2522%2522%252C%2522key%2522%253A%2522%2522%252C%2522isColumn%2522%253Afalse%252C%2522type%2522%253A%2522%2522%252C%2522isJSON%2522%253Afalse%257D%252C%2522timeAggregation%2522%253A%2522rate%2522%252C%2522spaceAggregation%2522%253A%2522sum%2522%252C%2522functions%2522%253A%255B%255D%252C%2522filters%2522%253A%257B%2522items%2522%253A%255B%255D%252C%2522op%2522%253A%2522AND%2522%257D%252C%2522expression%2522%253A%2522A%2522%252C%2522disabled%2522%253Afalse%252C%2522stepInterval%2522%253A60%252C%2522having%2522%253A%255B%255D%252C%2522limit%2522%253Anull%252C%2522orderBy%2522%253A%255B%257B%2522columnName%2522%253A%2522timestamp%2522%252C%2522order%2522%253A%2522desc%2522%257D%255D%252C%2522groupBy%2522%253A%255B%255D%252C%2522legend%2522%253A%2522%2522%257D%255D%252C%2522queryFormulas%2522%253A%255B%255D%257D%252C%2522promql%2522%253A%255B%257B%2522name%2522%253A%2522A%2522%252C%2522query%2522%253A%2522%2522%252C%2522legend%2522%253A%2522%2522%252C%2522disabled%2522%253Afalse%257D%255D%252C%2522clickhouse_sql%2522%253A%255B%257B%2522name%2522%253A%2522A%2522%252C%2522legend%2522%253A%2522%2522%252C%2522disabled%2522%253Afalse%252C%2522query%2522%253A%2522%2522%257D%255D%252C%2522id%2522%253A%252256693f86-d661-4335-9653-699fdefe8675%2522%257D&options=%7B%22selectColumns%22%3A%5B%7B%22key%22%3A%22serviceName%22%2C%22dataType%22%3A%22string%22%2C%22type%22%3A%22tag%22%2C%22isColumn%22%3Atrue%2C%22isJSON%22%3Afalse%2C%22id%22%3A%22serviceName--string--tag--true%22%2C%22isIndexed%22%3Afalse%7D%2C%7B%22key%22%3A%22name%22%2C%22dataType%22%3A%22string%22%2C%22type%22%3A%22tag%22%2C%22isColumn%22%3Atrue%2C%22isJSON%22%3Afalse%2C%22id%22%3A%22name--string--tag--true%22%2C%22isIndexed%22%3Afalse%7D%2C%7B%22key%22%3A%22durationNano%22%2C%22dataType%22%3A%22float64%22%2C%22type%22%3A%22tag%22%2C%22isColumn%22%3Atrue%2C%22isJSON%22%3Afalse%2C%22id%22%3A%22durationNano--float64--tag--true%22%2C%22isIndexed%22%3Afalse%7D%2C%7B%22key%22%3A%22httpMethod%22%2C%22dataType%22%3A%22string%22%2C%22type%22%3A%22tag%22%2C%22isColumn%22%3Atrue%2C%22isJSON%22%3Afalse%2C%22id%22%3A%22httpMethod--string--tag--true%22%2C%22isIndexed%22%3Afalse%7D%2C%7B%22key%22%3A%22responseStatusCode%22%2C%22dataType%22%3A%22string%22%2C%22type%22%3A%22tag%22%2C%22isColumn%22%3Atrue%2C%22isJSON%22%3Afalse%2C%22id%22%3A%22responseStatusCode--string--tag--true%22%2C%22isIndexed%22%3Afalse%7D%5D%2C%22maxLines%22%3A2%2C%22format%22%3A%22raw%22%2C%22fontSize%22%3A%22small%22%7D)

## Middleware Configuration

The API configures middleware in the following order:

1. **Static Files**: `app.UseStaticFiles()`
2. **HTTPS Redirection**: `app.UseHttpsRedirection()`
3. **CORS**: `app.UseCors("AllowedOriginsPolicy")`
4. **Error Handling**: Custom `ErrorHandlingMiddleware`
5. **Request Logging**: Custom `RequestLoggingMiddleware`

## Endpoint Registration

Endpoints are registered using a custom extension system:

1. Service registration: `builder.Services.AddEndpoints(typeof(IEndpoint))`
2. Endpoint mapping: `app.UseEndpoints()`

This automatically discovers and registers all classes implementing `IEndpoint`.

## Service Registration

Services are registered in both global and endpoint-specific contexts:

### Global Services (Program.cs)

- Database connections
- Health checks
- OpenTelemetry
- Core services

### Endpoint-Specific Services

Each endpoint class implements `MapServices()` to register its dependencies:

```csharp
public void MapServices(IServiceCollection services)
{
    services.TryAddScoped<IProfileRepository, ProfileRepository>();
    services.TryAddTransient<IValidator<GetFilteredProfilesRequest>, ProfileValidator>();
    // ...
}
```

## Health Check Configuration

The API configures comprehensive health checks for:

- API service health (`ApiHealthCheck`)
- Database connectivity (`DbHealthCheck`)
- Custom publisher for telemetry integration

## Database Connection

The API uses Dapper with a custom connection factory:

```csharp
services.AddSingleton<IDbConnectionFactory>(_ =>
    new DbConnectionFactory(configuration.GetConnectionString("DatabaseConnection")!));
```
