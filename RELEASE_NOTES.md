# Release v1.1.0

## Overview

This release enhances the SchoolPortal API with support for multi-year data analysis, improving the flexibility of the average successes endpoint. The API now allows querying exam results across multiple school years, enabling more comprehensive historical data analysis and comparison.

## New Features

- Added support for querying multiple school years in the institution average successes endpoint

## Improvements, Fixes & Technical Changes

- Implemented SQL table-valued parameters for efficient multi-year data retrieval
- Enhanced validation for arrays of school years including error messaging
- Updated integration tests to verify multi-year query functionality

# API Changes

- The **/api/v1/institutions/{institutionId}/average-successes** endpoint now accepts multiple schoolYear query parameters:

```
  GET /api/v1/institutions/2546/average-successes?schoolYear=2020&schoolYear=2021&schoolYear=2022&grade=7
```

<div style="margin: 24px 0; padding: 8px 0;">
    <hr style="height: 3px; background-color:rgb(21, 57, 156); border: none;">
</div>

# Release v1.0.0 - Initial Release

## Overview

This initial release introduces the SchoolPortal API, offering a comprehensive platform for accessing educational data. It provides core functionality for retreaving of data for school profiles and institutions, complemented by robust infrastructure components including health checks, telemetry, and advanced error handling.

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
