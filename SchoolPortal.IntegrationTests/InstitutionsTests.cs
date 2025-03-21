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

    public Task DisposeAsync() => Task.CompletedTask;

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
        var schoolYears = new[] { 2023, 2024 };
        var grade = 7;
        var subjectAbbreviationBEL = "БЕЛ";
        var subjectAbbreviationMAT = "MAT";
        var examAbbreviation = "НВО";

        var yearData = new Dictionary<int, (int BelCount, decimal BelScore, int MatCount, decimal MatScore)>
        {
            { 2023, (64, 59.17M, 43, 33.71M) },
            { 2024, (78, 62.43M, 56, 41.25M) }
        };

        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var examAbbreviationId = await dataSeeder.SeedExamAbbreviation(examAbbreviation);

        var institutionId = await dataSeeder.SeedInstitution(4565, "Test Institution6", "TI6");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        foreach (var year in schoolYears)
        {
            var schoolYearId = await dataSeeder.SeedSchoolYear(year);
            var subjectAbbreviation1Id = await dataSeeder.SeedSubjectAbbreviation(subjectAbbreviationBEL);
            var subjectAbbreviation2Id = await dataSeeder.SeedSubjectAbbreviation(subjectAbbreviationMAT);

            var (belCount, belScore, matCount, matScore) = yearData[year];

            await dataSeeder.SeedExamResults(
                belCount, belScore, grade, subjectAbbreviation1Id,
                subInstitutionId, schoolYearId, examAbbreviationId);

            await dataSeeder.SeedExamResults(
                matCount, matScore, grade, subjectAbbreviation2Id,
                subInstitutionId, schoolYearId, examAbbreviationId);
        }

        var queryParameters = string.Join("&", schoolYears.Select(y => $"schoolYear={y}")) + $"&grade={grade}";

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{queryParameters}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var examResultsResponse = await response.Content.ReadFromJsonAsync<GetExamResultsResponse>();
        Assert.NotNull(examResultsResponse);
        Assert.NotNull(examResultsResponse.ExamResults);
        Assert.NotEmpty(examResultsResponse.ExamResults);

        Assert.Equal(4, examResultsResponse.ExamResults.Count);
        Assert.Equal(4, examResultsResponse.ExamResultsCount);

        Assert.Contains(subjectAbbreviationBEL, examResultsResponse.ExamResults.Select(x => x.SubjectAbbreviation));
        Assert.Contains(subjectAbbreviationMAT, examResultsResponse.ExamResults.Select(x => x.SubjectAbbreviation));

        Assert.Contains(schoolYears[0], examResultsResponse.ExamResults.Select(x => x.SchoolYear));
        Assert.Contains(schoolYears[1], examResultsResponse.ExamResults.Select(x => x.SchoolYear));

        Assert.All(examResultsResponse.ExamResults, examResult =>
        {
            Assert.Equal(examAbbreviation, examResult.ExamAbbreviation);
            Assert.Equal(grade, examResult.Grade);
            Assert.Equal(subInstitutionId, examResult.InstitutionId);
        });

        foreach (var year in schoolYears)
        {
            var (expectedBelCount, expectedBelScore, expectedMatCount, expectedMatScore) = yearData[year];

            var belResultForYear = examResultsResponse.ExamResults.FirstOrDefault(x =>
                x.SubjectAbbreviation == subjectAbbreviationBEL && x.SchoolYear == year);
            var matResultForYear = examResultsResponse.ExamResults.FirstOrDefault(x =>
                x.SubjectAbbreviation == subjectAbbreviationMAT && x.SchoolYear == year);

            Assert.NotNull(belResultForYear);
            Assert.NotNull(matResultForYear);

            // Check year-specific values
            Assert.Equal(expectedBelCount, belResultForYear.CandidateCount);
            Assert.Equal(expectedBelScore, belResultForYear.AverageScore);
            Assert.Equal(expectedMatCount, matResultForYear.CandidateCount);
            Assert.Equal(expectedMatScore, matResultForYear.AverageScore);
        }
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsEmptyCollection_WhenExamResultsDoNotExist()
    {
        // Arrange
        var schoolYears = new[] { 2023, 2024 };
        var grade = 7;
        var settlement = "Софи";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);

        foreach (var year in schoolYears)
        {
            await dataSeeder.SeedSchoolYear(year);
        }
        var institutionId = await dataSeeder.SeedInstitution(231261, "Test Institution7", "TI7");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        var queryParameters = string.Join("&", schoolYears.Select(y => $"schoolYear={y}")) + $"&grade={grade}";

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

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ValidatesAllYears_WhenSomeYearsAreInvalid()
    {
        // Arrange
        var validYear = 2023;
        var invalidYear = 3000;
        var grade = 7;

        var settlement = "София";
        var neighbourhood = "Лозенец";
        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var institutionId = await dataSeeder.SeedInstitution(657485, "Test Institution12", "TI12");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);
        var schoolYearId = await dataSeeder.SeedSchoolYear(validYear);

        var queryParameters = $"schoolYear={validYear}&schoolYear={invalidYear}&grade={grade}";

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{queryParameters}");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResponse);
        Assert.NotNull(errorResponse.Errors);

        Assert.Contains($"Provided year ({invalidYear}) must be between",
                      errorResponse.Errors.First(e => e.Contains(invalidYear.ToString())));
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_HandlesMultipleYears_WhenRequestingManyYears()
    {
        // Arrange
        var manyYears = Enumerable.Range(2019, 5).ToArray(); // 2019, 2020, 2021, 2022, 2023
        var grade = 7;
        var settlement = "София";
        var neighbourhood = "Лозенец";
        var examAbbreviation = "НВО";
        var subjectAbbreviation = "БЕЛ";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var examAbbreviationId = await dataSeeder.SeedExamAbbreviation(examAbbreviation);
        var subjectAbbreviationId = await dataSeeder.SeedSubjectAbbreviation(subjectAbbreviation);
        var institutionId = await dataSeeder.SeedInstitution(924513, "Test Institution Many Years", "TIMY");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        foreach (var (year, index) in manyYears.Select((y, i) => (y, i)))
        {
            var schoolYearId = await dataSeeder.SeedSchoolYear(year);
            await dataSeeder.SeedExamResults(
                50 + index,
                60.0M + index,
                grade,
                subjectAbbreviationId,
                subInstitutionId,
                schoolYearId,
                examAbbreviationId);
        }

        var queryParameters = string.Join("&", manyYears.Select(y => $"schoolYear={y}")) + $"&grade={grade}";

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{queryParameters}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var examResultsResponse = await response.Content.ReadFromJsonAsync<GetExamResultsResponse>();
        Assert.NotNull(examResultsResponse);
        Assert.Equal(manyYears.Length, examResultsResponse.ExamResultsCount);

        foreach (var year in manyYears)
        {
            var yearResults = examResultsResponse.ExamResults.Where(r => r.SchoolYear == year);
            Assert.NotEmpty(yearResults);
        }

        var orderedResults = examResultsResponse.ExamResults.OrderBy(r => r.SchoolYear).ToArray();
        for (int i = 1; i < orderedResults.Length; i++)
        {
            Assert.True(orderedResults[i].AverageScore > orderedResults[i - 1].AverageScore,
                "Average scores should increase with each year");
            Assert.True(orderedResults[i].CandidateCount > orderedResults[i - 1].CandidateCount,
                "Candidate counts should increase with each year");
        }
    }
}
