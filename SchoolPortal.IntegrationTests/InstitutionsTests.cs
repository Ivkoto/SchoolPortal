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

    /// <summary>
    /// This test specifically targets the parameter binding.
    /// The test runs the same endpoint multiple times to ensure consistent behavior with parameter mapping.
    /// </summary>
    [Fact]
    public async Task GetInstitutionProfiles_ConsistentBehavior_WhenRunMultipleTimes()
    {
        // Arrange
        var schoolYear = 2024;
        var grade = 7;
        var institutionExternalId = 999999;
        var institutionFullName = "Parameter Test Institution";
        var institutionShortName = "PTI";
        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var institutionId = await dataSeeder.SeedInstitution(institutionExternalId, institutionFullName, institutionShortName);
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);
        
        var queryParameters = $"schoolYear={schoolYear}&grade={grade}";

        // Act & Assert - Run the same request multiple times to ensure consistency
        for (int i = 0; i < 5; i++)
        {
            var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/profiles?{queryParameters}");
            
            // This should consistently return OK status, not intermittent 500 errors
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var profilesResponse = await response.Content.ReadFromJsonAsync<GetFilteredProfilesResponse>();
            Assert.NotNull(profilesResponse);
            Assert.Equal(0, profilesResponse.ProfilesCount);
            Assert.NotNull(profilesResponse.Profiles);
            Assert.Empty(profilesResponse.Profiles);
        }
    }

    /// <summary>
    /// Test to verify that QueryMultipleAsync properly handles the stored procedure's multiple result sets
    /// and works consistently even when run alongside other tests that use the same stored procedure.
    /// </summary>
    [Fact]
    public async Task GetInstitutionProfiles_ProperMultipleResultSetHandling_WhenStoredProcedureReturnsThreeResultSets()
    {
        // Arrange
        var schoolYear = 2024;
        var grade = 7;
        var institutionExternalId = 888888;
        var institutionFullName = "MultiResultSet Test Institution";
        var institutionShortName = "MRSTI";
        var profileName = "MultiResultSet Test Profile";
        var profileType = "професионална";
        var profileExternalId = 777777;
        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var institutionId = await dataSeeder.SeedInstitution(institutionExternalId, institutionFullName, institutionShortName);
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);
        var profileId = await dataSeeder.SeedProfile(profileName, profileType, grade, subInstitutionId);
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

        // Verify that we get the expected profile back
        var profile = profilesResponse.Profiles.First();
        Assert.Equal(profileName, profile.ProfileName);
        Assert.Equal(profileType, profile.ProfileType);
        Assert.Equal(grade, profile.Grade);
        Assert.Equal(subInstitutionId, profile.InstitutionId);
        Assert.Equal(schoolYear, profile.SchoolYear);
        
        // Ensure the ProfilesCount is correctly set (this should come from the API, not the stored procedure's result sets)
        Assert.Equal(1, profilesResponse.ProfilesCount);
        Assert.Equal(profilesResponse.Profiles.Count, profilesResponse.ProfilesCount);
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
            errorResponse.Errors.First(e => e.Contains(invalidYear.ToString()))
        );
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
                examAbbreviationId
            );
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

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsBadRequest_WhenSchoolYearArrayIsEmpty()
    {
        // Arrange
        var institutionId = 12345;

        // Act - calling endpoint without any schoolYear parameters
        var response = await httpClient.GetAsync($"/api/v1/institutions/{institutionId}/average-successes");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResponse);
        Assert.NotNull(errorResponse.Errors);
        Assert.Contains("At least one school year must be provided",
            errorResponse.Errors.First(e => e.Contains("school year")));
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsAllGrades_WhenGradeParameterIsNotProvided()
    {
        // Arrange
        var schoolYears = new[] { 2023, 2024 };
        var grade7 = 7;
        var grade10 = 10;
        var subjectAbbreviationBEL = "БЕЛ";
        var examAbbreviation = "НВО";

        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var examAbbreviationId = await dataSeeder.SeedExamAbbreviation(examAbbreviation);
        var subjectAbbreviationId = await dataSeeder.SeedSubjectAbbreviation(subjectAbbreviationBEL);

        var institutionId = await dataSeeder.SeedInstitution(12345, "Test Institution Optional Grade", "TIOG");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        // Seed data for multiple grades across multiple years
        foreach (var year in schoolYears)
        {
            var schoolYearId = await dataSeeder.SeedSchoolYear(year);

            await dataSeeder.SeedExamResults(
                50, 65.5M, grade7, subjectAbbreviationId,
                subInstitutionId, schoolYearId, examAbbreviationId);

            await dataSeeder.SeedExamResults(
                40, 72.3M, grade10, subjectAbbreviationId,
                subInstitutionId, schoolYearId, examAbbreviationId);
        }

        var queryParameters = string.Join("&", schoolYears.Select(y => $"schoolYear={y}"));

        // Act
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{queryParameters}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var examResultsResponse = await response.Content.ReadFromJsonAsync<GetExamResultsResponse>();
        Assert.NotNull(examResultsResponse);
        Assert.NotNull(examResultsResponse.ExamResults);
        Assert.NotEmpty(examResultsResponse.ExamResults);

        Assert.Equal(4, examResultsResponse.ExamResultsCount);
        Assert.Equal(4, examResultsResponse.ExamResults.Count);

        var grade7Results = examResultsResponse.ExamResults.Where(r => r.Grade == grade7);
        var grade10Results = examResultsResponse.ExamResults.Where(r => r.Grade == grade10);

        Assert.Equal(2, grade7Results.Count());
        Assert.Equal(2, grade10Results.Count());

        Assert.Contains(schoolYears[0], examResultsResponse.ExamResults.Select(x => x.SchoolYear));
        Assert.Contains(schoolYears[1], examResultsResponse.ExamResults.Select(x => x.SchoolYear));

        Assert.All(examResultsResponse.ExamResults, examResult =>
        {
            Assert.Equal(examAbbreviation, examResult.ExamAbbreviation);
            Assert.Equal(subjectAbbreviationBEL, examResult.SubjectAbbreviation);
            Assert.Equal(subInstitutionId, examResult.InstitutionId);
            Assert.True(examResult.Grade == grade7 || examResult.Grade == grade10);
        });
    }    

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsResultsWithMixedGrades_WhenGradeParameterIsOptional()
    {
        // Arrange
        var schoolYears = new[] { 2023 };
        var grade4 = 4;
        var grade7 = 7;
        var grade12 = 12;
        var subjectAbbreviation = "БЕЛ";
        var examAbbreviation = "НВО";

        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var examAbbreviationId = await dataSeeder.SeedExamAbbreviation(examAbbreviation);
        var subjectAbbreviationId = await dataSeeder.SeedSubjectAbbreviation(subjectAbbreviation);

        var institutionId = await dataSeeder.SeedInstitution(98765, "Mixed Grades Institution", "MGI");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYears[0]);

        await dataSeeder.SeedExamResults(30, 55.0M, grade4, subjectAbbreviationId, subInstitutionId, schoolYearId, examAbbreviationId);
        await dataSeeder.SeedExamResults(50, 65.0M, grade7, subjectAbbreviationId, subInstitutionId, schoolYearId, examAbbreviationId);
        await dataSeeder.SeedExamResults(25, 75.0M, grade12, subjectAbbreviationId, subInstitutionId, schoolYearId, examAbbreviationId);

        var queryParameters = string.Join("&", schoolYears.Select(y => $"schoolYear={y}"));

        // Act - request without grade parameter to get all grades
        var response = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{queryParameters}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var examResultsResponse = await response.Content.ReadFromJsonAsync<GetExamResultsResponse>();
        Assert.NotNull(examResultsResponse);
        Assert.Equal(3, examResultsResponse.ExamResultsCount);

        var grades = examResultsResponse.ExamResults.Select(r => r.Grade).Distinct().OrderBy(g => g).ToArray();
        Assert.Equal(new[] { grade4, grade7, grade12 }, grades);

        var grade4Result = examResultsResponse.ExamResults.First(r => r.Grade == grade4);
        var grade7Result = examResultsResponse.ExamResults.First(r => r.Grade == grade7);
        var grade12Result = examResultsResponse.ExamResults.First(r => r.Grade == grade12);

        Assert.Equal(55.0M, grade4Result.AverageScore);
        Assert.Equal(65.0M, grade7Result.AverageScore);
        Assert.Equal(75.0M, grade12Result.AverageScore);
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ComparesBehavior_WithAndWithoutGradeParameter()
    {
        // Arrange
        var schoolYears = new[] { 2023 };
        var targetGrade = 7;
        var otherGrade = 10;
        var subjectAbbreviation = "БЕЛ";
        var examAbbreviation = "НВО";

        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var examAbbreviationId = await dataSeeder.SeedExamAbbreviation(examAbbreviation);
        var subjectAbbreviationId = await dataSeeder.SeedSubjectAbbreviation(subjectAbbreviation);

        var institutionId = await dataSeeder.SeedInstitution(11111, "Comparison Test Institution", "CTI");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);

        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYears[0]);

        await dataSeeder.SeedExamResults(40, 68.5M, targetGrade, subjectAbbreviationId, subInstitutionId, schoolYearId, examAbbreviationId);
        await dataSeeder.SeedExamResults(35, 73.2M, otherGrade, subjectAbbreviationId, subInstitutionId, schoolYearId, examAbbreviationId);

        var baseQuery = string.Join("&", schoolYears.Select(y => $"schoolYear={y}"));

        // Act 1 - Request with specific grade
        var responseWithGrade = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{baseQuery}&grade={targetGrade}");

        // Act 2 - Request without grade (should return all grades)
        var responseWithoutGrade = await httpClient.GetAsync($"/api/v1/institutions/{subInstitutionId}/average-successes?{baseQuery}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, responseWithGrade.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseWithoutGrade.StatusCode);

        var resultsWithGrade = await responseWithGrade.Content.ReadFromJsonAsync<GetExamResultsResponse>();
        var resultsWithoutGrade = await responseWithoutGrade.Content.ReadFromJsonAsync<GetExamResultsResponse>();

        Assert.NotNull(resultsWithGrade);
        Assert.NotNull(resultsWithoutGrade);

        // With grade: should return only 1 result (specific grade)
        Assert.Equal(1, resultsWithGrade.ExamResultsCount);
        Assert.Equal(targetGrade, resultsWithGrade.ExamResults.First().Grade);
        Assert.Equal(68.5M, resultsWithGrade.ExamResults.First().AverageScore);

        // Without grade: should return 2 results (all grades)
        Assert.Equal(2, resultsWithoutGrade.ExamResultsCount);
        Assert.Contains(resultsWithoutGrade.ExamResults, r => r.Grade == targetGrade && r.AverageScore == 68.5M);
        Assert.Contains(resultsWithoutGrade.ExamResults, r => r.Grade == otherGrade && r.AverageScore == 73.2M);
    }
}
