using System.Net;
using System.Net.Http.Json;
using SchoolPortal.Api.Models;
using SchoolPortal.IntegrationTests.Fixtures;

namespace SchoolPortal.IntegrationTests;

[Collection("IntegrationTestsCollection")]
public class InstitutionsTests : IAsyncLifetime, IClassFixture<SchoolPortalApiApplicationFactory<Program>>
{
    private readonly HttpClient httpClient;
    private readonly DatabaseFixture dbFixture;
    private readonly TestDataSeeder dataSeeder;


    public InstitutionsTests(SchoolPortalApiApplicationFactory<Program> factory, DatabaseFixture dbFixture)
    {
        this.httpClient = factory.CreateClient();
        this.dbFixture = dbFixture;
        this.dataSeeder = new TestDataSeeder(dbFixture);
    }

    public async Task InitializeAsync() => await dbFixture.ResetDatabaseAsync();

    public Task DisposeAsync() =>  Task.CompletedTask;

    [Fact]
    public async Task GetInstitutionById_ReturnsInstitution_WhenInstitutionExists()
    {
        // Arrange
        var externalId = 263493;
        var fullName = "Test Institution";
        var shortName = "TI";
        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);

        var institutionId = await dataSeeder.SeedInstitution(externalId, fullName, shortName);
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var institution = await response.Content.ReadFromJsonAsync<InstitutionModel>();
        Assert.NotNull(institution);
        Assert.Equal(subInstitutionId, institution.InstitutionId);
        Assert.Equal(fullName, institution.FullName);
        Assert.Equal(shortName, institution.ShortName);
    }

    [Fact]
    public async Task GetInstitutionById_ReturnsNotFound_WhenInstitutionDoesNotExist()
    {
        // Arrange
        var nonExistentInstitutionId = int.MaxValue;

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{nonExistentInstitutionId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResponse);
        Assert.Equal(404, errorResponse.ErrorCode);
        Assert.Equal($"No Institution found with the ID {nonExistentInstitutionId}", errorResponse.Message);
        Assert.Contains($"No Institution found with the ID {nonExistentInstitutionId}", errorResponse.Errors!);
    }

    [Fact]
    public async Task GetInstitutionProfiles_ReturnsProfiles_WhenProfilesExist()
    {
        // Arrange
        var schoolYear = 2024;
        var grade = 7;
        var institutionExternalId = 734614;
        var institutionFullName = "Test Institution2";
        var institutionShortName = "TI2";
        var profileName = "Test Profile";
        var profileType = "професионална";
        var profileGrade = 7;
        var profileExternalId = 122334;
        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var institutionId = await dataSeeder.SeedInstitution(institutionExternalId, institutionFullName, institutionShortName);
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);
        var profileId = await dataSeeder.SeedProfile(profileName, profileType, profileGrade, subInstitutionId);
        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);

        await dataSeeder.SeedProfileDetails(profileExternalId, profileId, schoolYearId);

        var queryParameters = $"schoolYear={schoolYear}&grade={grade}";

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/profiles?{queryParameters}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profilesResponse = await response.Content.ReadFromJsonAsync<GetFilteredProfilesResponse>();
        Assert.NotNull(profilesResponse);
        Assert.NotNull(profilesResponse.Profiles);
        Assert.NotEmpty(profilesResponse.Profiles);

        var profile = profilesResponse.Profiles.First();
        Assert.Equal(profileName, profile.ProfileName);
        Assert.Equal(profileType, profile.ProfileType);
        Assert.Equal(grade, profile.Grade);
        Assert.Equal(subInstitutionId, profile.InstitutionId);
        Assert.Equal(schoolYear, profile.SchoolYear);
    }

    [Fact]
    public async Task GetInstitutionProfiles_ReturnsEmptyCollection_WhenProfilesDoNotExist()
    {
        // Arrange
        var schoolYear = 2024;
        var grade = 10;
        var notExistingInstitutionId = 23452367;
        var queryParameters = $"schoolYear={schoolYear}&grade={grade}";

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{notExistingInstitutionId}/profiles?{queryParameters}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profilesResponse = await response.Content.ReadFromJsonAsync<GetFilteredProfilesResponse>();
        Assert.NotNull(profilesResponse);
        Assert.Equal(0, profilesResponse.ProfilesCount);
        Assert.NotNull(profilesResponse.Profiles);
        Assert.Empty(profilesResponse.Profiles);
    }    

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsExamResults_WhenExamResultsExist()
    {
        // Arrange
        var schoolYear = 2024;
        var grade = 7;
        var subjectAbbreviationBEL = "БЕЛ";
        var subjectAbbreviationMAT = "MAT";
        var examAbbreviation = "НВО";
        var candidateCountBEL = 64;
        var averageScoreBEL = 59.17M;
        var candidateCountMAT = 43;
        var averageScoreMAT = 33.71M;
        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);
        var institutionId = await dataSeeder.SeedInstitution(981265, "Test Institution6", "TI6");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);
        var examAbbreviation1Id = await dataSeeder.SeedExamAbbreviation(examAbbreviation);
        var subjectAbbreviation1Id = await dataSeeder.SeedSubjectAbbreviation(subjectAbbreviationBEL);
        var subjectAbbreviation2Id = await dataSeeder.SeedSubjectAbbreviation(subjectAbbreviationMAT);

        await dataSeeder.SeedExamResults(candidateCountBEL, averageScoreBEL, grade, subjectAbbreviation1Id, subInstitutionId, schoolYearId, examAbbreviation1Id);
        await dataSeeder.SeedExamResults(candidateCountMAT, averageScoreMAT, grade, subjectAbbreviation2Id, subInstitutionId, schoolYearId, examAbbreviation1Id);

        var queryParameters = $"schoolYear={schoolYear}&grade={grade}";

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{queryParameters}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var examResultsResponse = await response.Content.ReadFromJsonAsync<GetExamResultsResponse>();
        Assert.NotNull(examResultsResponse);
        Assert.NotNull(examResultsResponse.ExamResults);
        Assert.NotEmpty(examResultsResponse.ExamResults);

        Assert.Equal(2, examResultsResponse.ExamResults.Count);

        Assert.Contains(subjectAbbreviationBEL, examResultsResponse.ExamResults.Select(x => x.SubjectAbbreviation));
        Assert.Contains(subjectAbbreviationMAT, examResultsResponse.ExamResults.Select(x => x.SubjectAbbreviation));

        Assert.All(examResultsResponse.ExamResults, examResult =>
        {
            Assert.Equal(examAbbreviation, examResult.ExamAbbreviation);
            Assert.Equal(grade, examResult.Grade);
            Assert.Equal(schoolYear, examResult.SchoolYear);
            Assert.Equal(subInstitutionId, examResult.InstitutionId);
        });

        var belResult = examResultsResponse.ExamResults.FirstOrDefault(x => x.SubjectAbbreviation == subjectAbbreviationBEL);
        var matResult = examResultsResponse.ExamResults.FirstOrDefault(x => x.SubjectAbbreviation == subjectAbbreviationMAT);

        Assert.NotNull(belResult);
        Assert.NotNull(matResult);

        Assert.Equal(candidateCountBEL, belResult.CandidateCount);
        Assert.Equal(averageScoreBEL, belResult.AverageScore);
        Assert.Equal(candidateCountMAT, matResult.CandidateCount);
        Assert.Equal(averageScoreMAT, matResult.AverageScore);
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsEmptyCollection_WhenExamResultsDoNotExist()
    {
        // Arrange
        var schoolYear = 2024;
        var grade = 7;
        var settlement = "Софи";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);

        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);
        var institutionId = await dataSeeder.SeedInstitution(231261, "Test Institution7", "TI7");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        var queryParameters = $"schoolYear={schoolYear}&grade={grade}";

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{queryParameters}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var examResultsResponse = await response.Content.ReadFromJsonAsync<GetExamResultsResponse>();
        Assert.NotNull(examResultsResponse);
        Assert.NotNull(examResultsResponse.ExamResults);
        Assert.Equal(0, examResultsResponse.ExamResultsCount);
        Assert.Empty(examResultsResponse.ExamResults);
    }
}
