using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using SchoolPortal.Api.Endpoints;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;

namespace SchoolPortal.UnitTests.Endpoints
{
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
                .Setup(v => v.ValidateAsync(request.GeoLocationFilter, default))
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
            result
                .Should().BeOfType<Ok<GetFilteredProfilesResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetFilteredProfilesResponse>)?.Value;

            response.Should().NotBeNull();
            response!.Profiles.Should().BeEquivalentTo(profilesResponse);
            response.ProfilesCount.Should().Be(1);
            response.PageNumber.Should().Be(1);
            response.PageSize.Should().Be(1);
            response.TotalPages.Should().Be(1);
        }

        [Theory]
        [InlineData("ProfileType", "Invalid profile type.")]
        [InlineData("GeoLocation", "Invalid geo-location data.")]
        public async Task GetFilteredProfiles_ReturnsBadRequest_WhenValidationFails(string propertyName, string errorMessage)
        {
            // Arrange
            var request = CreateProfilesRequest();
            var validationFailures = new[] { new ValidationFailure(propertyName, errorMessage) };

            if (propertyName == "ProfileType")
            {
                SetupValidators(request, new ValidationResult(validationFailures), new ValidationResult());
            }
            else
            {
                SetupValidators(request, new ValidationResult(), new ValidationResult(validationFailures));
            }

            // Act
            var result = await profilesEndpoint.GetFilteredProfiles(
                httpContextMock.Object,
                request,
                profileRepositoryMock.Object,
                filtersValidatorMock.Object,
                locationValidatorMock.Object
             );

            // Assert
            result
                .Should().BeOfType<ProblemHttpResult>()
                .Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var validationDetails = (result as ProblemHttpResult)?.ProblemDetails as HttpValidationProblemDetails;

            validationDetails.Should().NotBeNull();
            validationDetails!.Title.Should().Be("One or more validation errors occurred.");

            validationDetails.Errors.Should().ContainKey(propertyName)
                .WhoseValue.Should().ContainSingle()
                .Which.Should().Be(errorMessage);
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
            result
                .Should().BeOfType<Ok<GetFilteredProfilesResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetFilteredProfilesResponse>)?.Value;

            response.Should().NotBeNull();
            response!.Profiles.Should().BeEmpty();
            response.ProfilesCount.Should().Be(0);
            response.PageNumber.Should().Be(1);
            response.PageSize.Should().Be(0);
            response.TotalPages.Should().Be(1);
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
            result
                .Should().BeOfType<Ok<ProfileModel>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseProfile = (result as Ok<ProfileModel>)?.Value;

            responseProfile.Should().NotBeNull();
            responseProfile!.ProfileId.Should().Be(profileId);
            responseProfile.ProfileName.Should().Be("Test Profile");
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
            result
                .Should().BeOfType<Ok<GetSciencesResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetSciencesResponse>)?.Value;

            response.Should().NotBeNull();
            response!.SciencesCount.Should().Be(sciences.Count);
            response.Sciences.Should().BeEquivalentTo(sciences);
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
            result
                .Should().BeOfType<Ok<GetSciencesResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetSciencesResponse>)?.Value;

            response.Should().NotBeNull();
            response!.SciencesCount.Should().Be(0);
            response.Sciences.Should().BeEmpty();
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
            result
                .Should().BeOfType<Ok<GetProfessionalDirectionsResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetProfessionalDirectionsResponse>)?.Value;

            response.Should().NotBeNull();
            response!.ProfessionalDirectionsCount.Should().Be(response.ProfessionalDirectionsCount);
            response.ProfessionalDirections.Should().BeEquivalentTo(professionalDirections);
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
            result
                .Should().BeOfType<Ok<GetProfessionalDirectionsResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);


            var response = (result as Ok<GetProfessionalDirectionsResponse>)?.Value;

            response.Should().NotBeNull();
            response!.ProfessionalDirectionsCount.Should().Be(0);
            response.ProfessionalDirections.Should().BeEmpty();
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
            result
                .Should().BeOfType<Ok<GetProfessionsResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetProfessionsResponse>)?.Value;

            response.Should().NotBeNull();
            response!.ProfessionsCount.Should().Be(expectedProfessions.Count);
            response.Professions.Should().BeEquivalentTo(expectedProfessions);
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
            result.Should()
                .BeOfType<Ok<GetProfessionsResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetProfessionsResponse>)?.Value;

            response.Should().NotBeNull();
            response!.ProfessionsCount.Should().Be(0);
            response.Professions.Should().BeEmpty();
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
            result.Should()
                .BeOfType<Ok<GetSpecialtiesResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);


            var response = (result as Ok<GetSpecialtiesResponse>)?.Value;

            response.Should().NotBeNull();
            response!.SpecialtesCount.Should().Be(expectedSpecialties.Count);
            response.Specialties.Should().BeEquivalentTo(expectedSpecialties);
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
            result
                .Should().BeOfType<Ok<GetSpecialtiesResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetSpecialtiesResponse>)?.Value;

            response.Should().NotBeNull();
            response!.SpecialtesCount.Should().Be(0);
            response.Specialties.Should().BeEmpty();
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
            result
                .Should().BeOfType<Ok<GetExamStagesScoresResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetExamStagesScoresResponse>)?.Value;

            response.Should().NotBeNull();
            response!.StagesCount.Should().Be(expectedScores.Count);
            response.ExamStageScores.Should().BeEquivalentTo(expectedScores);
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
            result
                .Should().BeOfType<Ok<GetExamStagesScoresResponse>>()
                .Which.StatusCode.Should().Be(StatusCodes.Status200OK);

            var response = (result as Ok<GetExamStagesScoresResponse>)?.Value;

            response.Should().NotBeNull();
            response!.StagesCount.Should().Be(0);
            response.ExamStageScores.Should().BeEmpty();
        }
    }
}
