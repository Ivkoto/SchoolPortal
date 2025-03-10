using System.Net;
using System.Net.Http.Json;
using SchoolPortal.Api.Models;
using SchoolPortal.IntegrationTests.Fixtures;

namespace SchoolPortal.IntegrationTests;

[Collection("IntegrationTestsCollection")]
public class ValidationTests : IAsyncLifetime, IClassFixture<SchoolPortalApiApplicationFactory<Program>>
{
    private readonly HttpClient httpClient;
    private readonly DatabaseFixture dbFixture;
    private readonly TestDataSeeder dataSeeder;

    public ValidationTests(SchoolPortalApiApplicationFactory<Program> factory, DatabaseFixture dbFixture)
    {
        this.httpClient = factory.CreateClient();
        this.dbFixture = dbFixture;
        this.dataSeeder = new TestDataSeeder(dbFixture);
    }

    public async Task InitializeAsync() => await dbFixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;    

    [Fact]
    public async Task GetInstitutionProfiles_ReturnsBadRequestWithMessage_WhenQueryParametersAreOutOfRange()
    {
        // Arrange
        var invalidSchoolYear = 2085;
        var invalidGrade = 6;
        var institutionExternalId = 634311;
        var institutionFullName = "Test Institution5";
        var institutionShortName = "TI5"; var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);

        var institutionId = await dataSeeder.SeedInstitution(institutionExternalId, institutionFullName, institutionShortName);
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        var queryParameters = $"schoolYear={invalidSchoolYear}&grade={invalidGrade}";

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/profiles?{queryParameters}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResponse);
        Assert.NotNull(errorResponse.Errors);
        Assert.Contains($"SchoolYear: Provided year ({invalidSchoolYear}) must be between 2010 and 2030, inclusive.", errorResponse.Errors);
        Assert.Contains("Grade: Must be one of the following: 4, 7, 10, 12", errorResponse.Errors);
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsBadRequestWithMessage_WhenQueryParametersAreOutOfRange()
    {
        // Arrange
        var invalidSchoolYears = new[] { 2056, 2057 };
        var invalidGrade = 6;
        var institutionExternalId = 3214521;
        var institutionFullName = "Test Institution8";
        var institutionShortName = "TI8";
        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);

        var institutionId = await dataSeeder.SeedInstitution(institutionExternalId, institutionFullName, institutionShortName);
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        var queryParameters = string.Join("&",
            invalidSchoolYears.Select(year => $"schoolYear={year}"))
            + $"&grade={invalidGrade}";

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{queryParameters}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResponse);
        Assert.NotNull(errorResponse.Errors);

        foreach (var invalidYear in invalidSchoolYears)
        {
            Assert.Contains($"SchoolYear: Provided year ({invalidYear}) must be between 2010 and 2030, inclusive.", errorResponse.Errors);
        }

        Assert.Contains("Grade: Must be one of the following: 4, 7, 10, 12", errorResponse.Errors);
    }

    [Fact]
    public async Task GetFilteredProfiles_ReturnsBadRequestWithMessage_WhenRequestIsInvalid()
    {
        // Arrange
        var ivnalidSchoolYear = 2075;
        var invalidRequest = new GetFilteredProfilesRequest(ivnalidSchoolYear, 6, null, null, null, null, null, null, null, null, 1, 10);

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/v1/profiles/lookup", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResponse);
        Assert.NotNull(errorResponse.Errors);
        Assert.Contains($"SchoolYear.SchoolYear: Provided year ({ivnalidSchoolYear}) must be between 2010 and 2030, inclusive.", errorResponse.Errors);
        Assert.Contains("Settlement: Must be София", errorResponse.Errors);
        Assert.Contains("Grade.Grade: Must be one of the following: 4, 7, 10, 12", errorResponse.Errors);
    }

    [Fact]
    public async Task GetFilteredProfiles_ReturnsBadRequestWithMessage_WhenGeoLocationIsInvalid()
    {
        // Arrange
        var invalidGeoLocation = new GeoLocationModel(100, 200, -1);
        var invalidRequest = new GetFilteredProfilesRequest(2024, 7, "София", null, invalidGeoLocation, null, null, null, null, null, 1, 10);

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/v1/profiles/lookup", invalidRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResponse);
        Assert.NotNull(errorResponse.Errors);
        Assert.Contains("Latitude: Latitude must be between -90 and 90 degrees.", errorResponse.Errors);
        Assert.Contains("Longitude: Longitude must be between -180 and 180 degrees.", errorResponse.Errors);
        Assert.Contains("Radius: Radius value cannot be a negative number.", errorResponse.Errors);
    }
}

