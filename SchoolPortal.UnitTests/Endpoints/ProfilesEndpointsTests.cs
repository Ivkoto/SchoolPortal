using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using SchoolPortal.Api.Endpoints;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;
using SchoolPortal.UnitTests.Validation;

namespace SchoolPortal.UnitTests.Endpoints;

public class ProfilesEndpointsTests
{
    private readonly Mock<IProfileRepository> profileRepositoryMock;
    private readonly Mock<IValidator<GetFilteredProfilesRequest>> filtersValidatorMock;
    private readonly Mock<IValidator<GeoLocationModel>> locationValidatorMock;
    private readonly Mock<HttpContext> httpContextMock;
    private readonly Profiles profilesEndpoint;

    public ProfilesEndpointsTests()
    {
        profileRepositoryMock = new();
        filtersValidatorMock = new();
        locationValidatorMock = new();
        profilesEndpoint = new();
        httpContextMock = new();

        httpContextMock.SetupGet(c => c.Response.Headers).Returns(new HeaderDictionary());
    }

    private void SetupValidators(GetFilteredProfilesRequest request, ValidationResult filtersValidationResult, ValidationResult locationValidationResult)
    {
        filtersValidatorMock
            .Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(filtersValidationResult);

        locationValidatorMock
            .Setup(v => v.ValidateAsync(request.GeoLocationFilter!, default))
            .ReturnsAsync(locationValidationResult);
    }

    private GetFilteredProfilesRequest CreateProfilesRequest()
        => new(
            SchoolYear: 2024,
            Grade: 7,
            Settlement: "София",
            Neighbourhood: null,
            GeoLocationFilter: new GeoLocationModel(42.69158343249817m, 23.326981836601483m, 1m),
            ProfileType: null,
            SpecialtyId: null,
            ProfessionId: null,
            ProfessionalDirectionId: null,
            ScienceId: null,
            PageNumber: null,
            PageSize: null
        );

    [Fact]
    public async Task GetFilteredProfiles_ReturnsOk_WhenRequestIsValid()
    {
        //Arrange
        var request = CreateProfilesRequest();
        var profilesResponse = new List<ProfileModel> { new ProfileModel() };

        SetupValidators(request, new ValidationResult(), new ValidationResult());

        profileRepositoryMock
            .Setup(repo => repo.GetFilteredProfiles(request))
            .ReturnsAsync((profilesResponse, TotalPages: 1));

        // Act
        var result = await profilesEndpoint.GetFilteredProfiles(
            httpContextMock.Object,
            request,
            profileRepositoryMock.Object,
            filtersValidatorMock.Object,
            locationValidatorMock.Object);

        // Assert
        Assert.IsType<Ok<GetFilteredProfilesResponse>>(result);
        var okResult = result as Ok<GetFilteredProfilesResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(profilesResponse, response!.Profiles);
        Assert.Equal(1, response.ProfilesCount);
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(1, response.PageSize);
        Assert.Equal(1, response.TotalPages);
    }    

    [Fact]
    public async Task GetFilteredProfiles_ReturnsEmptyProfiles_WhenNoMatch()
    {
        // Arrange
        var request = CreateProfilesRequest();
        var emptyProfilesResponse = new List<ProfileModel>();

        SetupValidators(request, new ValidationResult(), new ValidationResult());

        profileRepositoryMock
            .Setup(repo => repo.GetFilteredProfiles(request))
            .ReturnsAsync((emptyProfilesResponse, TotalPages: 1));

        // Act
        var result = await profilesEndpoint.GetFilteredProfiles(
            httpContextMock.Object,
            request,
            profileRepositoryMock.Object,
            filtersValidatorMock.Object,
            locationValidatorMock.Object);

        // Assert
        Assert.IsType<Ok<GetFilteredProfilesResponse>>(result);
        var okResult = result as Ok<GetFilteredProfilesResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Empty(response!.Profiles);
        Assert.Equal(0, response.ProfilesCount);
        Assert.Equal(1, response.PageNumber);
        Assert.Equal(0, response.PageSize);
        Assert.Equal(1, response.TotalPages);
    }


    [Fact]
    public async Task GetProfileById_ReturnsOK_WhenProfileExists()
    {
        // Arrange
        var profileId = 123;
        var profile = new ProfileModel { ProfileId = profileId, ProfileName = "Test Profile" };

        profileRepositoryMock
            .Setup(repo => repo.GetProfileById(profileId))
            .ReturnsAsync(profile);

        // Act
        var result = await profilesEndpoint.GetProfileById(
            profileId,
            httpContextMock.Object,
            profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<ProfileModel>>(result);
        var okResult = result as Ok<ProfileModel>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var responseProfile = okResult?.Value;

        Assert.NotNull(responseProfile);
        Assert.Equal(profileId, responseProfile!.ProfileId);
        Assert.Equal("Test Profile", responseProfile.ProfileName);
    }

    [Fact]
    public async Task GetSciences_ReturnsOK_WhenSciencesExist()
    {
        // Arrange
        var sciences = new List<ScienceModel>
        {
            It.IsAny<ScienceModel>(),
            It.IsAny<ScienceModel>()
        };

        profileRepositoryMock
            .Setup(repo => repo.GetAllSciences())
            .ReturnsAsync(sciences);

        // Act
        var result = await profilesEndpoint.GetSciences(httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetSciencesResponse>>(result);
        var okResult = result as Ok<GetSciencesResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(sciences.Count, response!.SciencesCount);
        Assert.Equal(sciences, response.Sciences);
    }

    [Fact]
    public async Task GetSciences_ReturnsEmptyList_WhenNoSciencesExist()
    {
        // Arrange
        var sciences = new List<ScienceModel>();

        profileRepositoryMock
            .Setup(repo => repo.GetAllSciences())
            .ReturnsAsync(sciences);

        // Act
        var result = await profilesEndpoint.GetSciences(httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetSciencesResponse>>(result);
        var okResult = result as Ok<GetSciencesResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(0, response!.SciencesCount);
        Assert.Empty(response.Sciences);
    }

    [Fact]
    public async Task GetProfessionalDirections_ReturnsOk_WhenDirectionsExist()
    {
        // Arrange
        var scienceId = 1;
        var professionalDirections = new List<ProfessionalDirectionModel>
        {
            It.IsAny<ProfessionalDirectionModel>(),
            It.IsAny<ProfessionalDirectionModel>()
        };

        profileRepositoryMock
            .Setup(repo => repo.GetProfessionalDirectionsByScienceId(scienceId))
            .ReturnsAsync(professionalDirections);

        // Act
        var result = await profilesEndpoint.GetProfessionalDirections(scienceId, httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetProfessionalDirectionsResponse>>(result);
        var okResult = result as Ok<GetProfessionalDirectionsResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(professionalDirections.Count, response!.ProfessionalDirectionsCount);
        Assert.Equal(professionalDirections, response.ProfessionalDirections);
    }

    [Fact]
    public async Task GetProfessionalDirections_ReturnsEmptyList_WhenNoDirectionsFound()
    {
        // Arrange
        var scienceId = 1;
        var expectedDirections = new List<ProfessionalDirectionModel>();

        profileRepositoryMock
            .Setup(repo => repo.GetProfessionalDirectionsByScienceId(scienceId))
            .ReturnsAsync(expectedDirections);

        // Act
        var result = await profilesEndpoint.GetProfessionalDirections(scienceId, httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetProfessionalDirectionsResponse>>(result);
        var okResult = result as Ok<GetProfessionalDirectionsResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(0, response!.ProfessionalDirectionsCount);
        Assert.Empty(response.ProfessionalDirections);
    }

    [Fact]
    public async Task GetProfessions_ReturnsOk_WhenProfessionsExist()
    {
        // Arrange
        var professionalDirectionId = 1;
        var expectedProfessions = new List<ProfessionModel>
        {
            It.IsAny<ProfessionModel>(),
            It.IsAny<ProfessionModel>()
        };

        profileRepositoryMock
            .Setup(repo => repo.GetProfessionsByProfessionalDirectionId(professionalDirectionId))
            .ReturnsAsync(expectedProfessions);

        // Act
        var result = await profilesEndpoint.GetProfessions(professionalDirectionId, httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetProfessionsResponse>>(result);
        var okResult = result as Ok<GetProfessionsResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(expectedProfessions.Count, response!.ProfessionsCount);
        Assert.Equal(expectedProfessions, response.Professions);
    }

    [Fact]
    public async Task GetProfessions_ReturnsEmptyList_WhenNoProfessionsFound()
    {
        // Arrange
        var professionalDirectionId = 1;
        var expectedProfessions = new List<ProfessionModel>();

        profileRepositoryMock
            .Setup(repo => repo.GetProfessionsByProfessionalDirectionId(professionalDirectionId))
            .ReturnsAsync(expectedProfessions);

        // Act
        var result = await profilesEndpoint.GetProfessions(professionalDirectionId, httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetProfessionsResponse>>(result);
        var okResult = result as Ok<GetProfessionsResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(0, response!.ProfessionsCount);
        Assert.Empty(response.Professions);
    }

    [Fact]
    public async Task GetSpecialties_ReturnsOk_WhenSpecialtiesExist()
    {
        // Arrange
        var professionId = 1;
        var expectedSpecialties = new List<SpecialtyModel>
        {
            It.IsAny<SpecialtyModel>(),
            It.IsAny<SpecialtyModel>(),
        };

        profileRepositoryMock
            .Setup(repo => repo.GetSpecialtiesByProfessionId(professionId))
            .ReturnsAsync(expectedSpecialties);

        // Act
        var result = await profilesEndpoint.GetSpecialties(professionId, httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetSpecialtiesResponse>>(result);
        var okResult = result as Ok<GetSpecialtiesResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(expectedSpecialties.Count, response!.SpecialtesCount);
        Assert.Equal(expectedSpecialties, response.Specialties);
    }

    [Fact]
    public async Task GetSpecialties_ReturnsEmptyList_WhenNoSpecialtiesFound()
    {
        // Arrange
        var professionId = 1;
        var expectedSpecialties = new List<SpecialtyModel>();

        profileRepositoryMock
            .Setup(repo => repo.GetSpecialtiesByProfessionId(professionId))
            .ReturnsAsync(expectedSpecialties);

        // Act
        var result = await profilesEndpoint.GetSpecialties(professionId, httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetSpecialtiesResponse>>(result);
        var okResult = result as Ok<GetSpecialtiesResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(0, response!.SpecialtesCount);
        Assert.Empty(response.Specialties);
    }

    [Fact]
    public async Task GetExamStagesScores_ReturnsOk_WhenNoScoresExist()
    {
        // Arrange
        var profileId = 1;
        var schoolYear = 2024;
        var expectedScores = new List<ExamStageScoresModel>
        {
            It.IsAny<ExamStageScoresModel>(),
            It.IsAny<ExamStageScoresModel>()
        };

        profileRepositoryMock
            .Setup(repo => repo.GetAllExamStageScores(profileId, schoolYear))
            .ReturnsAsync(expectedScores);

        // Act
        var result = await profilesEndpoint.GetExamStagesScores(profileId, schoolYear, httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetExamStagesScoresResponse>>(result);
        var okResult = result as Ok<GetExamStagesScoresResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(expectedScores.Count, response!.StagesCount);
        Assert.Equal(expectedScores, response.ExamStageScores);
    }

    [Fact]
    public async Task GetExamStagesScores_ReturnsEmptyList_WhenNoScoresFound()
    {
        // Arrange
        var profileId = 1;
        var schoolYear = 2024;
        var expectedScores = new List<ExamStageScoresModel>();

        profileRepositoryMock
            .Setup(repo => repo.GetAllExamStageScores(profileId, schoolYear))
            .ReturnsAsync(expectedScores);

        // Act
        var result = await profilesEndpoint.GetExamStagesScores(profileId, schoolYear, httpContextMock.Object, profileRepositoryMock.Object);

        // Assert
        Assert.IsType<Ok<GetExamStagesScoresResponse>>(result);
        var okResult = result as Ok<GetExamStagesScoresResponse>;
        Assert.Equal(StatusCodes.Status200OK, okResult?.StatusCode);

        var response = okResult?.Value;

        Assert.NotNull(response);
        Assert.Equal(0, response!.StagesCount);
        Assert.Empty(response.ExamStageScores);
    }
}
