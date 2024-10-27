
# SchoolPortal API

## Overview

SchoolPortal API provides access to various functionalities of the SchoolPortal system, including profiles, institutions, exam results, and more. This README offers a quick start guide and links to detailed documentation.

## License

**This project is licensed under the Devocean Solutions License - see the [LICENSE.md](LICENSE) file for details.**

## Quick Start

### Installation

1. Clone the repository.
2. Install dependencies.
3. Run the application.

### Example Usage

Here’s a simple example of how to interact with the API:

```bash
curl -X POST https://eduapi.azurewebsites.net/profiles/lookup \

  -H "Content-Type: application/json" \
  -d '{
      "schoolYear": 2024,
      "grade": 8,
      "settlement": "София",
      "neighbourhood": null,
      "geoLocationFilter": {
          "latitude": 42.69158343249817,
          "longitude": 23.326981836601483,
          "radius": 1
      },
      "profileType": null,
      "specialtyId": null,
      "professionId": null,
      "professionalDirectionId": null,
      "scienceId": null
  }'
  ```

## Documentation

- [API Documentation](docs/API.md) - Detailed information on the API endpoints, request/response formats, and more.
- [Configuration](docs/CONFIGURATION.md) - Advanced configuration options (if applicable).
