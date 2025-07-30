using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using SchoolPortal.Api.Endpoints;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;

namespace SchoolPortal.UnitTests.Endpoints;

public class InstitutionsEndpointsTests
{
    private readonly Mock<IInstitutionRepository> institutionRepositoryMock;
    private readonly Institutions institutionsEndpoint;
    private readonly DefaultHttpContext httpContext;

    public InstitutionsEndpointsTests()
    {
        institutionRepositoryMock = new();
        institutionsEndpoint = new();
        httpContext = new();
    }

    [Fact]
    public async Task GetInstitutionById_ReturnsOk_WhenInstitutionExists()
    {
        // Arrange
        var institutionId = 1;

        var institution = new InstitutionModel(
            InstitutionId: institutionId,
            Kind: "Kind",
            Director: "Director",
            Websites: "Websites",
            Emails: "Emails",
            PhoneNumbers: "PhoneNumbers",
            AddressOfActivity: "AddressOfActivity",
            Area: "Area",
            Municipality: "Municipality",
            Region: "Region",
            Settlement: "Settlement",
            SettlementType: "SettlementType",
            Neighbourhood: "Neighbourhood",
            PostalCode: "PostalCode",
            GeoLatitude: 42.0m,
            GeoLongitide: 23.0m,
            ExternalId: 123,
            IsActive: true,
            EIK: "EIK",
            FullName: "Test Institution",
            ShortName: "ShortName",
            PreparationType: "PreparationType",
            InstStatus: "InstStatus",
            FinancingType: "FinancingType",
            InstOwnership: "InstOwnership"
        );

        institutionRepositoryMock
            .Setup(repo => repo.GetInstitutionById(institutionId))
            .ReturnsAsync(institution);

        // Act
        var result = await institutionsEndpoint.GetInstitutionById(
            institutionId,
            httpContext,
            institutionRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<InstitutionModel>>(result);

        var okResult = result as Ok<InstitutionModel>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var responseInstitution = okResult?.Value;

        Assert.NotNull(responseInstitution);
        Assert.Equal(institutionId, responseInstitution.InstitutionId);
    }

    [Fact]
    public async Task GetInstitutionProfiles_ReturnsOk_WhenProfilesExist()
    {
        // Arrange
        var institutionId = 1;
        var schoolYear = 2024;
        var grade = 7;

        var profiles = new List<ProfileModel>
        {
            It.IsAny<ProfileModel>(),
            It.IsAny<ProfileModel>()
        };

        institutionRepositoryMock
            .Setup(repo => repo.GetInstitutionProfiles(institutionId, schoolYear, grade))
            .ReturnsAsync(profiles);

        // Act
        var result = await institutionsEndpoint.GetInstitutionProfiles(
            institutionId,
            httpContext,
            schoolYear,
            grade,
            institutionRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetFilteredProfilesResponse>>(result);
        var okResult = result as Ok<GetFilteredProfilesResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(profiles.Count, response.ProfilesCount);
        Assert.Equal(profiles, response.Profiles);
    }

    [Fact]
    public async Task GetInstitutionProfiles_ReturnsEmptyList_WhenNoProfilesExist()
    {
        // Arrange
        var institutionId = 1;
        var schoolYear = 2024;
        var grade = 7;
        var profiles = new List<ProfileModel>();

        institutionRepositoryMock
            .Setup(repo => repo.GetInstitutionProfiles(institutionId, schoolYear, grade))
            .ReturnsAsync(profiles);

        // Act
        var result = await institutionsEndpoint.GetInstitutionProfiles(
            institutionId,
            httpContext,
            schoolYear,
            grade,
            institutionRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetFilteredProfilesResponse>>(result);
        var okResult = result as Ok<GetFilteredProfilesResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(0, response.ProfilesCount);
        Assert.Empty(response.Profiles);
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsOk_WhenExamResultsExist()
    {
        // Arrange
        var institutionId = 1;
        var schoolYears = new int[] { 2023, 2024 };
        var grade = 7;
        var examResults = new List<ExamResultModel>
        {
            It.IsAny<ExamResultModel>(),
            It.IsAny<ExamResultModel>()
        };

        institutionRepositoryMock
            .Setup(repo => repo.GetInstitutionAverageSuccesses(institutionId, schoolYears, grade))
            .ReturnsAsync(examResults);

        // Act
        var result = await institutionsEndpoint.GetInstitutionAverageSuccesses(
            institutionId,
            httpContext,
            schoolYears,
            grade,
            institutionRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetExamResultsResponse>>(result);
        var okResult = result as Ok<GetExamResultsResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(examResults.Count, response.ExamResultsCount);
        Assert.Equal(examResults, response.ExamResults); ;
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsEmptyList_WhenNoExamResultsExist()
    {
        // Arrange
        var institutionId = 1;
        var schoolYear = new int[] { 2023, 2024 };
        var grade = 7;
        var examResults = new List<ExamResultModel>();

        institutionRepositoryMock
            .Setup(repo => repo.GetInstitutionAverageSuccesses(institutionId, schoolYear, grade))
            .ReturnsAsync(examResults);

        // Act
        var result = await institutionsEndpoint.GetInstitutionAverageSuccesses(
            institutionId,
            httpContext,
            schoolYear,
            grade,
            institutionRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetExamResultsResponse>>(result);
        var okResult = result as Ok<GetExamResultsResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(0, response.ExamResultsCount);
        Assert.Empty(response.ExamResults);
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsOk_WhenGradeIsNull()
    {
        // Arrange
        var institutionId = 1;
        var schoolYears = new int[] { 2023, 2024 };
        int? grade = null; // Testing optional grade functionality
        var examResults = new List<ExamResultModel>
        {
            It.IsAny<ExamResultModel>(),
            It.IsAny<ExamResultModel>()
        };

        institutionRepositoryMock
            .Setup(repo => repo.GetInstitutionAverageSuccesses(institutionId, schoolYears, grade))
            .ReturnsAsync(examResults);

        // Act
        var result = await institutionsEndpoint.GetInstitutionAverageSuccesses(
            institutionId,
            httpContext,
            schoolYears,
            grade,
            institutionRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetExamResultsResponse>>(result);
        var okResult = result as Ok<GetExamResultsResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(examResults.Count, response.ExamResultsCount);
        Assert.Equal(examResults, response.ExamResults);
        
        // Verify repository was called with null grade
        institutionRepositoryMock.Verify(
            repo => repo.GetInstitutionAverageSuccesses(institutionId, schoolYears, null),
            Times.Once);
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ThrowsValidationException_WhenSchoolYearArrayIsEmpty()
    {
        // Arrange
        var institutionId = 1;
        var schoolYears = new int[] { };
        int? grade = 7;

        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () =>
            await institutionsEndpoint.GetInstitutionAverageSuccesses(
                institutionId,
                httpContext,
                schoolYears,
                grade,
                institutionRepositoryMock.Object));

        // Verify repository was never called due to validation failure
        institutionRepositoryMock.Verify(
            repo => repo.GetInstitutionAverageSuccesses(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<int?>()),
            Times.Never);
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ThrowsValidationException_WhenSchoolYearArrayIsNull()
    {
        // Arrange
        var institutionId = 1;
        int[] schoolYears = null!;
        int? grade = 7;

        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(async ()
            => await institutionsEndpoint.GetInstitutionAverageSuccesses(
                institutionId,
                httpContext,
                schoolYears,
                grade,
                institutionRepositoryMock.Object)
        );

        // Verify repository was never called due to validation failure
        institutionRepositoryMock.Verify(
            repo => repo.GetInstitutionAverageSuccesses(It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<int?>()),
            Times.Never);
    }
}
