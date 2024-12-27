using FluentAssertions;
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
        result
            .Should().BeOfType<Ok<InstitutionModel>>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

        var responseInstitution = (result as Ok<InstitutionModel>)?.Value;

        responseInstitution.Should().NotBeNull();
        responseInstitution!.InstitutionId.Should().Be(institutionId);
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
        result
            .Should().BeOfType<Ok<GetFilteredProfilesResponse>>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = (result as Ok<GetFilteredProfilesResponse>)?.Value;

        response.Should().NotBeNull();
        response!.ProfilesCount.Should().Be(profiles.Count);
        response.Profiles.Should().BeEquivalentTo(profiles);
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
        result
            .Should().BeOfType<Ok<GetFilteredProfilesResponse>>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = (result as Ok<GetFilteredProfilesResponse>)?.Value;

        response.Should().NotBeNull();
        response!.ProfilesCount.Should().Be(0);
        response.Profiles.Should().BeEmpty();
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsOk_WhenExamResultsExist()
    {
        // Arrange
        var institutionId = 1;
        var schoolYear = 2024;
        var grade = 7;
        var examResults = new List<ExamResultModel>
        {
            It.IsAny<ExamResultModel>(),
            It.IsAny<ExamResultModel>()
        };

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
        result
            .Should().BeOfType<Ok<GetExamResultsResponse>>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = (result as Ok<GetExamResultsResponse>)?.Value;

        response.Should().NotBeNull();
        response!.ExamResultsCount.Should().Be(examResults.Count);
        response.ExamResults.Should().BeEquivalentTo(examResults);
    }

    [Fact]
    public async Task GetInstitutionAverageSuccesses_ReturnsEmptyList_WhenNoExamResultsExist()
    {
        // Arrange
        var institutionId = 1;
        var schoolYear = 2024;
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
        result
            .Should().BeOfType<Ok<GetExamResultsResponse>>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

        var response = (result as Ok<GetExamResultsResponse>)?.Value;

        response.Should().NotBeNull();
        response!.ExamResultsCount.Should().Be(0);
        response.ExamResults.Should().BeEmpty();
    }
}
