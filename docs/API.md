<!-- Release v1.2.0: Updated API documentation for improved clarity -->

# SchoolPortal API Documentation

## Overview

This document provides documentation for the SchoolPortal API endpoints, request/response formats, and configuration. The API follows a .NET Minimal API approach with versioned endpoints.

## Table of Contents

- [Educational Profile Endpoints](#profile-endpoints)
- [Institution Endpoints](#institution-endpoints)
- [Location Endpoints](#location-endpoints)
- [Health Check Endpoints](#health-check-endpoints)
- [Validation Rules](#validation-rules)
- [CORS Policies](#cors-policies)
- [API Documentation](#api-documentation)
- [Error Handling](#error-handling)

## Educational Profile Endpoints

### `POST /api/v1/profiles/lookup`

- **Description**: Get filtered educational profiles based on criteria.

  **Note:** The pagination parameters `pageNumber` and `pageSize` are optional. If they are set to null, the endpoint returns all matching educational profiles without pagination.

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
  "pageNumber": 1,
  "pageSize": 10
}
```

- **Response:** A JSON array containing profile objects:

```json
{
  "profilesCount": 51,
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 6,
  "profiles": [
    {
      "profileId": 1799,
      "profileName": "Медицинска техника - НЕ",
      "profileType": "професионална",
      "grade": 7,
      "studyPeriod": "5",
      "institutionId": 2873,
      "institutionFullName": "НАЦИОНАЛНА ПРОФЕСИОНАЛНА ГИМНАЗИЯ ПО ПРЕЦИЗНА ТЕХНИКА И ОПТИКА \"М. В. Ломоносов\"",
      "institutionShortName": "НПГПТО \"М.В.Ломоносов\"",
      "gradingFormulas": "(2 * БЕЛ + 2 * МАТ) + (1 * БЕЛ + 1 * М)",
      "studyMethod": "разширено",
      "educationType": "дневна",
      "classesCount": 1.00,
      "firstForeignLanguage": "Немски език",
      "schoolYear": 2024,
      "isPaperOnly": false,
      "isClosed": false,
      "externalId": 317,
      "quotasTotal": 26,
      "quotasMale": null,
      "quotasFemale": null,
      "specialtyId": 161,
      "specialty": "Медицинска техника",
      "specialtyExternalId": 5210505,
      "professionalQualificationLevel": 3,
      "isProtected": false,
      "hasExpectedShortage": false,
      "isProfessional": true,
      "professionId": 68,
      "profession": "Техник на прецизна техника",
      "professionExternalId": 521050,
      "professionalDirectionId": 18,
      "professionalDirection": "Машиностроене, металообработване и металургия",
      "professionalDirectionExternalId": 521,
      "scienceId": 7,
      "science": "Техника",
      "scienceExternalId": 52,
      "area": "София-град",
      "settlement": "София",
      "region": "Възраждане",
      "neighbourhood": "София център"
    }
    ...more educational profiles...
  ]
}
```

- **Validation Rules**:
  - `schoolYear`: Must be between 2010 and 2030
  - `grade`: Must be one of the following: 4, 7, 10, 12
  - `geoLocationFilter.latitude`: Must be between -90 and 90
  - `geoLocationFilter.longitude`: Must be between -180 and 180
  - `geoLocationFilter.radius`: Must be between 0 and 100

### `GET /api/v1/profiles/{profileId}`

- **Description**: Get educational profile by ID
- **CORS**: AllowedOriginsPolicy
- **Path Parameters**:
  - `profileId`: The unique identifier of the profile
- **Response**: Single profile object
- **Error Responses**:
  - `404 Not Found`: No Profile found with the ID...

```json
{
  "profileId": 195,
  "profileName": "Софтуерни и хардуерни науки",
  "profileType": "профилирана",
  "grade": 7,
  "studyPeriod": "5",
  "institutionId": 243,
  "institutionFullName": "Средно училище \"Св. Св. Кирил и Методий\"",
  "institutionShortName": null,
  "gradingFormulas": "(2 * БЕЛ + 2 * МАТ) + (1 * БЕЛ + 1 * М)",
  "studyMethod": "разширено",
  "educationType": "дневна",
  "classesCount": 1.0,
  "firstForeignLanguage": "Английски език",
  "schoolYear": 2024,
  "isPaperOnly": false,
  "isClosed": false,
  "externalId": 267,
  "quotasTotal": 26,
  "quotasMale": null,
  "quotasFemale": null,
  "specialtyId": 6,
  "specialty": "Софтуерни и хардуерни науки",
  "specialtyExternalId": 80111,
  "professionalQualificationLevel": null,
  "isProtected": false,
  "hasExpectedShortage": false,
  "isProfessional": false,
  "professionId": null,
  "profession": null,
  "professionExternalId": null,
  "professionalDirectionId": null,
  "professionalDirection": null,
  "professionalDirectionExternalId": null,
  "scienceId": null,
  "science": null,
  "scienceExternalId": null,
  "area": "Бургас",
  "settlement": "Бургас",
  "region": "Приморие",
  "neighbourhood": "Център"
}
```

### `GET /api/v1/profiles/sciences`

- **Description**: Get all available sciences
- **CORS**: AllowedOriginsPolicy
- **Response**: List of science objects

```json
{
  "sciencesCount": 3,
  "sciences": [
    {
      "id": 6,
      "externalId": 48
      "name": "Информатика",
    },
    {
      "id": 7,
      "name": "Техника",
      "externalId": 52
    },
    ...more sciences...
  ]
}
```

### `GET /api/v1/profiles/professional-directions/{scienceId}`

- **Description**: Get professional directions by science ID
- **CORS**: AllowedOriginsPolicy
- **Path Parameters**:
  - `scienceId`: The unique identifier of the science
- **Response**: List of professional direction objects

```json
{
  "professionalDirectionsCount": 2,
  "professionalDirections": [
    {
      "id": 16,
      "externalId": 481,
      "name": "Компютърни науки",
      "scienceId": 6
    },
    {
      "id": 17,
      "externalId": 482,
      "name": "Приложна информатика",
      "scienceId": 6
    }
  ]
}
```

### `GET /api/v1/profiles/professions/{professionalDirectionId}`

- **Description**: Get professions by professional direction ID
- **CORS**: AllowedOriginsPolicy
- **Path Parameters**:
  - `professionalDirectionId`: The unique identifier of the professional direction
- **Response**: List of profession objects

```json
{
  "professionsCount": 5,
  "professions": [
    {
      "id": 55,
      "externalId": 481010,
      "name": "Програмист",
      "professionalDirectionId": 16
    },
    {
      "id": 56,
      "externalId": 481020,
      "name": "Системен програмист",
      "professionalDirectionId": 16
    }
    ...more professions...
  ]
}
```

### `GET /api/v1/profiles/specialties/{professionId}`

- **Description**: Get specialties by profession ID
- **CORS**: AllowedOriginsPolicy
- **Path Parameters**:
  - `professionId`: The unique identifier of the profession. **Use 0 to get profiled specialties.**
- **Response**: List of specialty objects

```json
{
  "specialtesCount": 1,
  "specialties": [
    {
      "id": 132,
      "externalId": 4810101,
      "name": "Програмно осигуряване",
      "isProfessional": true,
      "professionId": 55
    }
  ]
}
```

- **Special Case**: If `professionId` is 0, returns only "профилирани" specialties

```json
{
  "specialtesCount": 12,
  "specialties": [
    {
      "id": 1,
      "externalId": 0,
      "name": "Музика ПДМУ",
      "isProfessional": false,
      "professionId": 0
    },
    {
      "id": 2,
      "externalId": 80071,
      "name": "Чужди езици",
      "isProfessional": false,
      "professionId": 0
    }
    ...more specialties...
  ]
}
```

### `GET /api/v1/profiles/{profileId}/exam-stages/{schoolYear}`

- **Description**: Get exam stage scores for a educational profile
- **CORS**: AllowedOriginsPolicy
- **Path Parameters**:
  - `profileId`: The unique identifier of the profile
  - `schoolYear`: The academic year
- **Response**:

```json
{
  "stagesCount": 4,
  "examStageScores": [
    {
      "stageId": 5488,
      "stageNumber": 1,
      "freePositionsTotal": 26,
      "freePositionsMen": 0,
      "freePositionsWomen": 0,
      "isAggregatedScore": false,
      "profileId": 1761,
      "profileName": "Чужди езици -АЕ интензивно, ИЕ, ГИ и ИЦ",
      "schoolYear": 2024,
      "isClosed": false,
      "minTotalScore": 379.25,
      "minMaleScore": 379.25,
      "minFemaleScore": 379.25,
      "maxTotalScore": 446.75,
      "maxMaleScore": 446.75,
      "maxFemaleScore": 428.0
    },
    {
      "stageId": 5881,
      "stageNumber": 2,
      "freePositionsTotal": 0,
      "freePositionsMen": 0,
      "freePositionsWomen": 0,
      "isAggregatedScore": false,
      "profileId": 1761,
      "profileName": "Чужди езици -АЕ интензивно, ИЕ, ГИ и ИЦ",
      "schoolYear": 2024,
      "isClosed": false,
      "minTotalScore": 373.5,
      "minMaleScore": 373.5,
      "minFemaleScore": 376.0,
      "maxTotalScore": 446.75,
      "maxMaleScore": 446.75,
      "maxFemaleScore": 428.0
    },
    ...more stages....
  ]
}
```

## Institution Endpoints

### `GET /api/v1/institutions/{institutionId}`

- **Description**: Get institution details by ID
- **CORS**: AllowedOriginsPolicy
- **Path Parameters**:
  - `institutionId`: The unique identifier of the institution
- **Response**: Institution object with details

```json
{
  "institutionId": 256,
  "kind": "професионална гимназия",
  "director": "Елеонора Лилова",
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
  "geoLatitude": 42.52036998244305,
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

- **Error Responses**:
  - `404 Not Found`: No Institution found with the ID 2794567

### `GET /api/v1/institutions/{institutionId}/profiles`

- **Description**: Get institution educational profiles for specific school year and grade
- **CORS**: AllowedOriginsPolicy
- **Path Parameters**:
  - `institutionId`: The unique identifier of the institution
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

- **Validation Rules**:
  - `schoolYear`: Must be between 2010 and 2030
  - `grade`: Must be one of the following: 4, 7, 10, 12

### `GET /api/v1/institutions/{institutionId}/average-successes`

- **Description**: Get institution average exam success rates
- **CORS**: AllowedOriginsPolicy
- **Path Parameters**:
  - `institutionId`: The unique identifier of the institution
- **Query Parameters**:
  - `schoolYear` (required, multiple): Academic year(s)
  - `grade` (required): School grade
- **Response**: List of exam results with count

```json
{
  "examResultsCount": 2,
  "examResults": [
    {
      "schoolYear": 2022,
      "institutionId": 4010,
      "examAbbreviation": "НВО",
      "subjectName": "Български език и литература",
      "subjectAbbreviation": "БЕЛ",
      "preparationType": null,
      "candidateCount": 55,
      "averageScore": 59.43,
      "grade": 7,
      "levelType": null
    },
    {
      "schoolYear": 2022,
      "institutionId": 4010,
      "examAbbreviation": "НВО",
      "subjectName": "Математика",
      "subjectAbbreviation": "М",
      "preparationType": null,
      "candidateCount": 55,
      "averageScore": 27.39,
      "grade": 7,
      "levelType": null
    }
  ]
}
```

- **Validation Rules**:
  - `schoolYear`: Each year must be between 2010 and 2030
  - `grade`: Must be one of the following: 4, 7, 10, 12
- **Example Query with Multiple Years**:

```
GET /api/v1/institutions/256/average-successes?schoolYear=2022&schoolYear=2023&schoolYear=2024&grade=7
```

```json
{
  "examResultsCount": 6,
  "examResults": [
    {
      "schoolYear": 2022,
      "institutionId": 4010,
      "examAbbreviation": "НВО",
      "subjectName": "Български език и литература",
      "subjectAbbreviation": "БЕЛ",
      "preparationType": null,
      "candidateCount": 55,
      "averageScore": 59.43,
      "grade": 7,
      "levelType": null
    },
    {
      "schoolYear": 2023,
      "institutionId": 4010,
      "examAbbreviation": "НВО",
      "subjectName": "Български език и литература",
      "subjectAbbreviation": "БЕЛ",
      "preparationType": null,
      "candidateCount": 50,
      "averageScore": 58.81,
      "grade": 7,
      "levelType": null
    },
    {
      "schoolYear": 2024,
      "institutionId": 4010,
      "examAbbreviation": "НВО",
      "subjectName": "Български език и литература",
      "subjectAbbreviation": "БЕЛ",
      "preparationType": null,
      "candidateCount": 64,
      "averageScore": 59.17,
      "grade": 7,
      "levelType": null
    }
  ]
}
```

## Location Endpoints

### `GET /api/v1/location/neighbourhoods/{settlement}`

- **Description**: Get neighbourhoods by settlement
- **CORS**: AllowedOriginsPolicy
- **Path Parameters**:
  - `settlement`: The name of the settlement (city/town)
- **Response**: A JSON array containing neighbourhoods:

```json
{
  "neighbourhoodsCount": 92,
  "neighbourhoods": [
    {
      "Neighbourhood": "Лозенец"
    },
    {
      "Neighbourhood": "Изгрев"
    },
    ...more neighbourhoods...
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
  "totalDuration": "00:00:00.0823566",
  "entries": {
    "api_health": {
      "data": {},
      "description": "API is running",
      "duration": "00:00:00.0000171",
      "status": "Healthy",
      "tags": ["api"]
    },
    "database_health": {
      "data": {},
      "description": "Database connection is healthy",
      "duration": "00:00:00.0821958",
      "status": "Healthy",
      "tags": ["database"]
    }
  }
}
```

## Validation Rules

The API enforces consistent validation rules across endpoints:

### School Year Validation

- Valid years are between 2010 and 2030 (inclusive)
- Error message: "Provided year ({year}) must be between 2010 and 2030, inclusive."

### Grade Validation

- Valid grades are: 4, 7, 10, and 12
- Error message: "Must be one of the following: 4, 7, 10, 12"

### GeoLocation Validation

- Latitude: Between -90 and 90
- Longitude: Between -180 and 180
- Radius: Between 0 and 100 (kilometers)

## CORS Policies

The API implements two CORS policies:

1. AllowedOriginsPolicy:
   - Allowed Methods: GET, POST
   - Any Headers
   - 2 hour preflight cache
2. PaginationPolicy:
   - Same as AllowedOriginsPolicy
   - Additional Exposed Header: X-Pagination

## Error Handling

The API provides consistent error responses:

```json
{
  "errorCode": 400,
  "message": "Validation failed",
  "innerException": null,
  "errors": [
    "SchoolYear: Provided year (2075) must be between 2010 and 2030, inclusive.",
    "Grade: Must be one of the following: 4, 7, 10, 12"
  ]
}
```

Error codes:

- 400: Bad Request (validation failures)
- 404: Not Found (resource doesn't exist)
- 500: Internal Server Error (unhandled exceptions)

## API Documentation

### Swagger

- **Description**: Interactive API documentation interface
- **Links**:
  - PROD: https://eduapi.azurewebsites.net/swagger/index.html
  - DEV: https://eduapi-dev.azurewebsites.net/swagger/index.html
