# SchoolPortal API

## Overview

SchoolPortal API provides a comprehensive system for accessing educational data across Bulgarian schools. The API offers endpoints for querying school educational profiles, institutions, exam results, and other educational statistics, enabling applications to retrieve accurate and up-to-date information from the education system.

## License

**This project is licensed under the Devocean Solutions License - see the [LICENSE.md](LICENSE.md) file for details.**

## Features

- **Educational Profiles Management**: Search and filter educational profiles by various criteria
- **Institution Data**: Access information about educational institutions
- **Exam Results**: Retrieve examination statistics and historical data
- **Location-based Searches**: Find schools using geographic criteria
- **Comprehensive Metadata**: Access categorized data about sciences, professions, specialties with school year filtering
- **School Year-Aware Data**: Retrieve sciences and related metadata filtered by specific school years
- **Health Monitoring**: Built-in health check and monitoring endpoints

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server 2019 or newer
- Visual Studio 2022, VS Code, or other compatible IDE

## Quick Start

### Installation

1. Clone the repository:

   ```bash
   git clone https://DevOceanSolutions@dev.azure.com/DevOceanSolutions/EducationInfoPortal/_git/SchoolPortal
   ```

2. Navigate to the project directory:

   ```bash
   cd EducationInfoPortal/SchoolPortal
   ```

3. Set up the database:

   ```bash
   cd SchoolPortal.Database.Deploy
   dotnet run
   ```

4. Update the connection string in `appsettings.json` if needed.

5. Run the API:

   ```bash
   cd ../SchoolPortal.Api
   dotnet run
   ```

6. Access the API at `https://localhost:7154` or `http://localhost:5154`.

### Example Usage

Here's a simple example of how to interact with the API:

#### Lookup Educational Profiles with Geolocation

```bash
curl -X POST https://eduapi.azurewebsites.net/api/v1/profiles/lookup \
  -H "Content-Type: application/json" \
  -d '{
      "schoolYear": 2024,
      "grade": 7,
      "settlement": "София",
      "neighbourhood": null,
      "geoLocationFilter": {
          "latitude": 42.69158343249817,
          "longitude": 23.326981836601483,
          "radius": 1
      },
      "profileType": "професионална",
      "specialtyId": null,
      "professionId": null,
      "professionalDirectionId": null,
      "scienceId": null,
      "pageNumber": 1,
      "pageSize": 10
  }'
```

#### Get Institution Details

```bash
curl -X GET https://eduapi.azurewebsites.net/api/v1/institutions/123
```

#### Get Exam Results for Multiple Years

```bash
curl -X GET "https://eduapi.azurewebsites.net/api/v1/institutions/123/average-successes?schoolYear=2020&schoolYear=2021&schoolYear=2022&grade=7"
```

#### Get Sciences for Specific School Year

```bash
# Get sciences for a specific year
curl -X GET https://eduapi.azurewebsites.net/api/v1/profiles/sciences/2024

# Get sciences for current year (default)
curl -X GET https://eduapi.azurewebsites.net/api/v1/profiles/sciences
```

## Project Structure

- **SchoolPortal.Api**: Main API project
- **SchoolPortal.Database.Deploy**: Database deployment scripts
- **SchoolPortal.UnitTests**: Unit tests
- **SchoolPortal.IntegrationTests**: Integration tests
- **docs**: Documentation files

## API Documentation

- [API Documentation](docs/API.md) - Detailed API endpoints, request/response formats
- [Configuration](docs/CONFIGURATION.md) - Configuration options and settings
- [Architecture](docs/ARCHITECTURE.md) - Solution architecture and design patterns
- [Release Notes](RELEASE_NOTES.md) - Latest releases, features, improvements, and fixes

## Development Environment Setup

1. Install the required prerequisites.
2. Clone the repository and open in your preferred IDE.
3. Create a local copy of `appsettings.Development.json` with your development settings.
4. Run the database deployment project to set up your local database.
5. Start the API project and navigate to `/swagger` for interactive API documentation.

## Running Tests

```bash
# Run unit tests
dotnet test SchoolPortal.UnitTests

# Run integration tests
dotnet test SchoolPortal.IntegrationTests
```

## Deployment

The API is deployed in development & production environments:

- **Development**: https://eduapi-dev.azurewebsites.net
- **Production**: https://eduapi.azurewebsites.net

## Support

For support or inquiries, please contact Devocean Solutions through our website:
[Devocean Services](https://devocean.services/)
