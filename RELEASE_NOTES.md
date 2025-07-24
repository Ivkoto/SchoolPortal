# Release v1.4.1

## Overview

This patch release updates the CORS configuration to include an additional allowed origin for improved cross-origin request support.

## Improvements, Fixes & Technical Changes

- **CORS Configuration**: Added new allowed origin `https://kandidatstvam.bg` to the CORS policy for enhanced cross-domain access support

## Technical Details

The following origin has been added to the allowed origins list:

- `https://kandidatstvam.bg`

This change enables the SchoolPortal API to accept cross-origin requests from the kandidatstvam.bg domain, maintaining the existing security policies including credential support and preflight caching.

# Release v1.4.0

## Overview

This release focuses on enhancing the development experience by resolving Swagger/OpenAPI compatibility issues and improving API documentation infrastructure. The primary improvement addresses protocol-specific (HTTP/HTTPS) rendering issues in Swagger UI, ensuring consistent API documentation access across different development environments and browsers.

## New Features

- Dynamic server URL detection for Swagger/OpenAPI documentation
- Enhanced Swagger UI configuration with improved developer experience features
- Protocol-agnostic OpenAPI specification generation

## Improvements, Fixes & Technical Changes

- **Swagger/OpenAPI**: Implemented dynamic server URL detection using PreSerializeFilters to resolve HTTPS/HTTP compatibility issues
- **Developer Experience**: Enhanced Swagger UI with request duration display, try-it-out enabled by default
- **CORS Configuration**: Added null-safety improvements with fallback to empty array for allowed origins
- **Documentation**: Improved OpenAPI metadata with contact information and enhanced descriptions
- **Package Updates**: Updated multiple NuGet packages to latest stable versions for improved performance and security

## Package Updates

### Major Updates

- **Swashbuckle.AspNetCore**: 7.2.0 → 9.0.1 (major update with OpenAPI 3.0.4 support)
- **Swashbuckle.AspNetCore.Annotations**: 7.2.0 → 9.0.1
- **FluentValidation**: Changed from FluentValidation.AspNetCore 11.3.0 to FluentValidation 12.0.0

### Minor Updates

- **Microsoft.AspNetCore.Mvc.Testing**: 9.0.2 → 9.0.6
- **Microsoft.Data.SqlClient**: 6.0.1 → 6.0.2
- **Microsoft.Extensions.Configuration**: 9.0.2 → 9.0.6
- **Microsoft.Extensions.Configuration.EnvironmentVariables**: 9.0.2 → 9.0.6
- **Microsoft.Extensions.Configuration.Json**: 9.0.2 → 9.0.6
- **Microsoft.NET.Test.Sdk**: 17.13.0 → 17.14.1
- **OpenTelemetry** (all packages): 1.11.1 → 1.12.0
- **Serilog.Sinks.Datadog.Logs**: 0.5.5 → 0.5.6
- **xunit.runner.visualstudio**: 3.0.2 → 3.1.1

## Technical Details

### Swagger/OpenAPI Documentation URLs

- **Production**: https://eduapi.azurewebsites.net/api-docs/index.html
- **Development**: https://eduapi-dev.azurewebsites.net/swagger/index.html
- **Local Development**:
  - HTTP: http://localhost:5141/swagger/index.html
  - HTTPS: https://localhost:7154/swagger/index.html

> The Swagger UI route is now dynamic and protocol-agnostic. In production, it is available at `/api-docs/index.html` (configurable via `Swagger:ProductionRoute` in appsettings). In development, the default route is `/swagger/index.html`. The server URL in the OpenAPI spec is set dynamically based on the current request context, ensuring correct protocol (HTTP/HTTPS) and host.

- Added `PreSerializeFilters` to dynamically set server URLs based on current request context
- Enhanced `SwaggerUIOptions` with developer-friendly defaults and improved configuration
- Added null-safe CORS origin handling to prevent runtime exceptions
- Updated NuGet packages to latest stable versions, including major Swashbuckle update for better OpenAPI 3.0.4 support
- Migrated from FluentValidation.AspNetCore to standalone FluentValidation package for improved compatibility

# Release v1.3.0

## Overview

This release introduces school year-aware science data retrieval and implements code quality improvements across the API. The GetSciences endpoint now supports filtering by school year, introducing a clean state of the Sciences, Professional Directions, Professions, Specialties for the current year or selected year - all these because we can face a different External IDs that come from MON for the same Sciences or Professional Directions, etc. every year. Minor refactoring has been applied to improve encapsulation and modernize collection handling throughout the codebase.

## New Features

- Enhanced GetSciences endpoint with school year filtering support
- Added school year parameter to sciences data model and database schema
- Implemented optional school year parameter with automatic fallback to current year

## Improvements, Fixes & Technical Changes

- **Code Quality**: Replaced `List<T>` with `IReadOnlyCollection<T>` across all response models for better encapsulation
- **Modern C#**: Migrated from `.ToList()` to collection expressions for improved performance and readability
- **Database Schema**: Added SchoolYearId foreign key relationship to Science table
- **API Enhancement**: GetSciences endpoint now accepts optional school year parameter
- **Test Coverage**: Updated unit and integration tests to support new school year functionality
- **Repository Pattern**: Enhanced repository methods to support school year-based queries

## API Changes

- The **/api/v1/profiles/sciences/{schoolYear}** endpoint now accepts an optional school year parameter:

```
  GET /api/v1/profiles/sciences/2024  # Get sciences for specific year
  GET /api/v1/profiles/sciences       # Get sciences for current year (default)
```

## Database Changes

- Added `SchoolYearId` column to `[Application].[Science]` table
- Updated `[Application].[uv_Sciences]` view to include school year information
- Modified `[Application].[usp_GetAllSciences]` stored procedure to filter by school year

<div style="margin: 24px 0; padding: 8px 0;">
    <hr style="height: 3px; background-color:rgb(21, 57, 156); border: none;">
</div>

# Release v1.2.0

## Overview

This release enhances the project documentation and reinforces centralized package management. Documentation files have been updated for clarity and consistency, and the Directory.Packages.props now centrally defines all package versions.

## New Features

- Updated project documentation (API, Architecture, Configuration, and Contributing)
- Centralized version definitions for NuGet packages

## Improvements, Fixes & Technical Changes

- Revised documentation content for improved clarity
- No modifications to licensing terms

# Release v1.1.0

## Overview

This release enhances the SchoolPortal API with support for multi-year data analysis, improving the flexibility of the average successes endpoint. The API now allows querying exam results across multiple school years, enabling more comprehensive historical data analysis and comparison.

## New Features

- Added support for querying multiple school years in the institution average successes endpoint

## Improvements, Fixes & Technical Changes

- Implemented SQL table-valued parameters for efficient multi-year data retrieval
- Enhanced validation for arrays of school years including error messaging
- Updated integration tests to verify multi-year query functionality

## API Changes

- The **/api/v1/institutions/{institutionId}/average-successes** endpoint now accepts multiple schoolYear query parameters:

```
  GET /api/v1/institutions/2546/average-successes?schoolYear=2020&schoolYear=2021&schoolYear=2022&grade=7
```

<div style="margin: 24px 0; padding: 8px 0;">
    <hr style="height: 3px; background-color:rgb(21, 57, 156); border: none;">
</div>

# Release v1.0.0 - Initial Release

## Overview

This initial release introduces the SchoolPortal API, offering a comprehensive platform for accessing educational data. It provides core functionality for retrieving data for school profiles and institutions, complemented by robust infrastructure components including health checks, telemetry, and advanced error handling.

## Core Features

- Foundational API endpoints that provide data for school profiles and their institutions.
- Comprehensive health check system for API and database monitoring
- OpenTelemetry integration enabling distributed tracing
- Robust middleware for centralized error handling and request logging

## Technical Implementation

- Complete suite of unit tests covering core functionality
- Input validation framework with detailed error feedback
- Performance monitoring and comprehensive logging infrastructure

## Known Issues

- Minor performance issues under heavy load remain
- Further logging enhancements are planned for upcoming releases
