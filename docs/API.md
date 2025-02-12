# SchoolPortal API Documentation

## Overview

This document provides documentation for the SchoolPortal API endpoints, request/response formats, and configuration. The API follows a .NET Minimal API approach with versioned endpoints.

## Table of Contents

- [Profile Endpoints](#profile-endpoints)
- [Institution Endpoints](#institution-endpoints)
- [Location Endpoints](#location-endpoints)
- [Health Check Endpoints](#health-check-endpoints)
- [CORS Policies](#cors-policies)
- [API Documentation](#api-documentation)

## Profile Endpoints

### `POST /api/v1/profiles/lookup`
- **Description**: Get filtered profiles based on criteria
- **CORS**: PaginationPolicy
- **Request Body**:
```json
{
  "schoolYear": 2024,
  "grade": 7,
  "settlement": "София",
  "neighbourhood": null,
  "geoLocationFilter": {
    "latitude": 42.69,
    "longitude": 23.33,
    "radius": 2
  },
  "profileType": "професионална",
  "specialtyId": null,
  "professionId": null,
  "professionalDirectionId": null,
  "scienceId": null,
  "pageNumber":1,
  "pageSize": 10
}
```
- **Response:** A JSON array containing profile objects:
```json
{
  "profilesCount": 10,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 6,
  "profiles": [
    {
      "ProfileId": 255,
      "ProfileName": "Компютърна техника и технологии",
      "Type": "професионална",
      "Grade": "7",
      "StudyPeriod": 5,
      "InstitutionId": 255,
      "GradingFormulas": "(2 * БЕЛ + 2 * МАТ) + (1 * БЕЛ + 1 * КМИТ)",
      "StudyMethod": "разширено",
      "EducationType": "дневна",
      "ClassesCount": 1.00,
      "FirstForeignLanguage": "Английски език",
      "SchoolYear": 2024,
      "IsPaperOnly": false,
      "ExternalId": 2420,
      "QuotasTotal": 26,
      "QuotasMale": 13,
      "QuotasFemale": 9,
      "Specialty": "Компютърни мрежи",
      "SpecialtyExternalId": 80121,
      "ProfessionalQualificationLevel": 3,
      "IsProtected": false,
      "HasExpectedShortage": false,
      "SpecialtyDescription": "РД 09 – 298/19.02.2009",
      "Profession": "Техник на компютърни системи",
      "ProfessionExternalId": 523050,
      "ProfessionalDirection": "Електроника, автоматика, комуникационна и компютърна техника",
      "ProfessionalDirectionExternalId": 523050,
      "Science": "Техника",
      "ScienceExternalId": "52"
    }
  ],
  "totalPages": 10,
  "currentPage": 1,
  "pageSize": 10,
  "totalCount": 100
}
```
### `GET /api/v1/profiles/{profileId}`
- **Description**: Get profile by ID
- **CORS**: AllowedOriginsPolicy
- **Response**: Single profile object

### `GET /api/v1/profiles/sciences`
- **Description**: Get all available sciences
- **CORS**: AllowedOriginsPolicy
- **Response**: List of science objects

### `GET /api/v1/profiles/professional-directions/{scienceId}`
- **Description**: Get professional directions by science ID
- **CORS**: AllowedOriginsPolicy
- **Response**: List of professional direction objects

### `GET /api/v1/profiles/{profileId}/exam-stages/{schoolYear}`
- **Description**: Get exam stage scores for a profile
- **CORS**: AllowedOriginsPolicy
- **Response**: 
```json
{
  "stages": [
    {
      "StageNumber": 1,
      "SchoolYear": 2024,
      "TotalPositions": 26,
      "MalePositions": null,
      "FemalePositions": null,
      "MinTotalScores": 82.50,
      "MinMaleScores": 127.00,
      "MinFemaleScores": 82.50,
      "MaxTotalScores": 219.00,
      "MaxMaleScores": 128.00,
      "MaxFemaleScores": 219.00
    }
  ]
}
```

## Institution Endpoints

### `GET /api/v1/institutions/{institutionId}`
- **Description**: Get institution details by ID
- **CORS**: AllowedOriginsPolicy
- **Response**: Institution object with details
```json
{
    "institutionId": 256,
    "kind": "професионална гимназия",
    "director": null,
    "websites": "https://www.pght-zelinskij.com/",
    "emails": "200247@edu.mon.bg;info-200247@edu.mon.bg",
    "phoneNumbers": "+35956881227;+359886385735",
    "addressOfActivity": "к-с Изгрев до бл. 53",
    "area": "Бургас",
    "municipality": "Бургас",
    "region": "Изгрев",
    "settlement": "Бургас",
    "settlementType": "град",
    "neighbourhood": "Изгрев",
    "postalCode": "8008",
    "geoLatitude": 42.520369982443050,
    "geoLongitide": 27.459555033759102,
    "externalId": 200247,
    "isActive": true,
    "eik": "000044192",
    "fullName": "Професионална гимназия по химични технологии \"Акад. Н. Д. Зелинский\"",
    "shortName": "ПГХТ \"Акад. Н. Д. Зелинский\"",
    "preparationType": "Неспециализирано",
    "instStatus": "действаща",
    "financingType": "Министерство на образованието и науката",
    "instOwnership": "Държавно"
}
```

### `GET /api/v1/institutions/{institutionId}/profiles`
- **Description**: Get institution profiles for specific school year and grade
- **CORS**: AllowedOriginsPolicy
- **Query Parameters**:
  - `schoolYear` (required): Academic year
  - `grade` (required): School grade
- **Response**: List of profiles with count
```json
{
    "profilesCount": 5,
    "profiles": [
        {
            "profileId": 255,
            "profileName": "Example Profile",
            ...
        }
    ]
}
```

### `GET /api/v1/institutions/{institutionId}/average-successes`
- **Description**: Get institution average exam success rates
- **CORS**: AllowedOriginsPolicy
- **Query Parameters**:
  - institutionId
  - academic year
  - grade
  - 
- **Response**: List of exam results with count
```json
{
  "examResultsCount": 2,
  "examResults": [
    {
      "SchoolYear": 2024,
      "InstitutionId": 1,
      "ExamAbbreviation": "MAT",
      "SubjectName": "Mathematics",
      "SubjectAbbreviation": "MATH",
      "PreparationType": "Standard",
      "CandidateCount": 100,
      "AverageScore": 85,
      "Grade": 7,
      "LevelType": "Advanced"
    }
  ]
}
```
## Location Endpoints

### `GET /api/v1/locations/{settlement}/neighbourhoods`
- **Description**: Get neighbourhoods by settlement
- **CORS**: AllowedOriginsPolicy
- **Response**: A JSON array containing neighbourhoods:
```json
{
  "neighbourhoods": [
    {
      "Neighbourhood": "Neighbourhood 1"
    },
    {
      "Neighbourhood": "Neighbourhood 2"
    }
  ]
}
```

## Health Check Endpoints

### `GET /api/v1/health`
- **Description**: Health check endpoint
- **Response**: A JSON object containing health check status:
```json
{
    "status": "Healthy",
    "totalDuration": "00:00:00.1067220",
    "entries": {
        "sqlserver": {
            "data": {},
            "duration": "00:00:00.0982388",
            "status": "Healthy",
            "tags": []
        }
    }
}
```

### `GET /api/v1/ping`
- **Description**: Simple connectivity check
- **CORS**: AllowedOriginsPolicy
- **Response**: 200 OK with "Pong!" if service is running

### `GET /api/v1/ping/details`
- **Description**: Detailed connectivity check including database
- **CORS**: AllowedOriginsPolicy
- **Response**:
    - 200 OK with "Connection to the database established" if successful
    - 500 Internal Server Error with "Could not connect to database" if failed

## CORS Policies
The API implements two CORS policies:

1. AllowedOriginsPolicy:
    - Allowed Methods: GET, POST
    - Any Headers
    - 2 hour preflight cache
2. PaginationPolicy:
    - Same as AllowedOriginsPolicy
    - Additional Exposed Header: X-Pagination

## API Documentation
### Swagger
- **Description**: Interactive API documentation interface
- **Links**:
    - PROD: https://eduapi.azurewebsites.net/swagger/index.html
    - DEV: https://eduapi-dev.azurewebsites.net/swagger/index.html