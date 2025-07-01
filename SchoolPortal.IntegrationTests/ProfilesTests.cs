using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using SchoolPortal.Api.Models;
using SchoolPortal.IntegrationTests.Fixtures;

namespace SchoolPortal.IntegrationTests;

[Collection("IntegrationTestsCollection")]
public class ProfilesTests : IAsyncLifetime, IClassFixture<SchoolPortalApiApplicationFactory<Program>>
{
    private readonly HttpClient httpClient;
    private readonly DatabaseFixture dbFixture;
    private readonly TestDataSeeder dataSeeder;

    public ProfilesTests(SchoolPortalApiApplicationFactory<Program> factory, DatabaseFixture dbFixture)
    {
        this.httpClient = factory.CreateClient();
        this.dbFixture = dbFixture;
        this.dataSeeder = new TestDataSeeder(dbFixture);
    }

    public async Task InitializeAsync() => await dbFixture.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetFilteredProfiles_ReturnsProfiles_WhenProfilesExist()
    {
        var schoolYear = 2024;
        var grade = 7;
        var profileName = "Test Profile";
        var profileType = "професионална";
        var profileExternalId = 82531;
        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);
        var institutionId = await dataSeeder.SeedInstitution(123456, "Test Institution", "TI");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);
        var profileId = await dataSeeder.SeedProfile(profileName, profileType, grade, subInstitutionId);
        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);
        await dataSeeder.SeedProfileDetails(profileExternalId, profileId, schoolYearId);

        // TODO @IvayloK - it can't pass if I provide profileType in the GetFilteredProfilesRequest! Check why!
        var request = new GetFilteredProfilesRequest(schoolYear, grade, settlement, neighbourhood, null, null, null, null, null, null, 1, 10);

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/v1/profiles/lookup", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profilesResponse = await response.Content.ReadFromJsonAsync<GetFilteredProfilesResponse>();
        Assert.NotNull(profilesResponse);
        Assert.NotEmpty(profilesResponse.Profiles);
        Assert.Equal(1, profilesResponse.ProfilesCount);

        var profile = profilesResponse.Profiles.First();
        Assert.Equal(profileName, profile.ProfileName);
        Assert.Equal(profileType, profile.ProfileType);
        Assert.Equal(grade, profile.Grade);
        Assert.Equal(subInstitutionId, profile.InstitutionId);
        Assert.Equal(schoolYear, profile.SchoolYear);
    }

    [Fact]
    public async Task GetFilteredProfiles_EmptyCollection_WhenProfilesDoNotExist()
    {
        // Arrange
        var request = new GetFilteredProfilesRequest(2024, 7, "София", null, null, null, null, null, null, null, null, null);

        // Act
        var response = await httpClient.PostAsJsonAsync("/api/v1/profiles/lookup", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profilesResponse = await response.Content.ReadFromJsonAsync<GetFilteredProfilesResponse>();

        Assert.NotNull(profilesResponse);
        Assert.Equal(0, profilesResponse.ProfilesCount);
        Assert.NotNull(profilesResponse.Profiles);
        Assert.Empty(profilesResponse.Profiles);
    }

    [Fact]
    public async Task GetProfileById_ReturnsProfile_WhenProfileExists()
    {
        // Arrange
        var profileName = "Test Profile12";
        var profileType = "професионална";
        var grade = 7;
        var settlement = "София";
        var neighbourhood = "Лозенец";

        var addressId = await dataSeeder.SeedNeighbourhood(settlement, neighbourhood);

        var institutionId = await dataSeeder.SeedInstitution(541232, "Test Institution12", "TI12");
        var subInstitutionId = await dataSeeder.SeedSubInstitution(institutionId, addressId);
        var profileId = await dataSeeder.SeedProfile(profileName, profileType, grade, subInstitutionId);

        // Act
        var response = await httpClient.GetAsync($"/api/v1/profiles/{profileId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var profile = await response.Content.ReadFromJsonAsync<ProfileModel>();
        Assert.NotNull(profile);
        Assert.Equal(profileName, profile.ProfileName);
        Assert.Equal(profileType, profile.ProfileType);
        Assert.Equal(grade, profile.Grade);
        Assert.Equal(subInstitutionId, profile.InstitutionId);
    }

    [Fact]
    public async Task GetProfileById_ReturnsNotFound_WhenProfileDoesNotExist()
    {
        // Arrange
        var nonExistentProfileId = int.MaxValue;

        // Act
        var response = await httpClient.GetAsync($"/api/v1/profiles/{nonExistentProfileId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(errorResponse);
        Assert.Equal(404, errorResponse.ErrorCode);
        Assert.Equal($"No Profile found with the ID {nonExistentProfileId}", errorResponse.Message);
        Assert.Contains($"No Profile found with the ID {nonExistentProfileId}", errorResponse.Errors!);
    }

    [Fact]
    public async Task GetSciences_ReturnsSciences_WhenSciencesExist()
    {
        // Arrange
        var schoolYear = 2024;
        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);
        var sciences = new List<(string Name, int ExternalId, int schoolYearId)>
        {
            ("Изкуства", 21, schoolYearId),
            ("Информатика", 48, schoolYearId),
            ("Ветеринарна медицина", 64, schoolYearId)
        };

        foreach (var science in sciences)
        {
            await dataSeeder.SeedScience(science.Name, science.ExternalId, science.schoolYearId);
        }

        // Act
        var response = await httpClient.GetAsync($"/api/v1/profiles/sciences/{schoolYear}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var sciencesResponse = await response.Content.ReadFromJsonAsync<GetSciencesResponse>();
        Assert.NotNull(sciencesResponse);
        Assert.NotEmpty(sciencesResponse.Sciences);
        Assert.Equal(sciences.Count(), sciencesResponse.SciencesCount);
        Assert.Contains(sciencesResponse.Sciences, n => n.Name == "Изкуства");
        Assert.Contains(sciencesResponse.Sciences, n => n.Name == "Информатика");
        Assert.Contains(sciencesResponse.Sciences, n => n.Name == "Ветеринарна медицина");
    }

    [Fact]
    public async Task GetSciences_ReturnsEmptyCollection_WhenThereAreNoSciences()
    {
        // Act
        var response = await httpClient.GetAsync("/api/v1/profiles/sciences");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var sciencesResponse = await response.Content.ReadFromJsonAsync<GetSciencesResponse>();
        Assert.NotNull(sciencesResponse);
        Assert.NotNull(sciencesResponse.Sciences);
        Assert.Equal(0, sciencesResponse.SciencesCount);
        Assert.Empty(sciencesResponse.Sciences);
    }

    [Fact]
    public async Task GetProfessionalDirections_ReturnsProfessionalDirections_WhenProfessionalDirectionsExist()
    {
        // Arrange
        var schoolYear = 2024;
        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);
        var science = (Name: "Информатика", ExternalId: 48);
        var professionalDirection = (Name: "Компютърни науки", ExternalId: 481);
        var scienceId = await dataSeeder.SeedScience(science.Name, science.ExternalId, schoolYearId);

        await dataSeeder.SeedProfessionalDirection(professionalDirection.Name, professionalDirection.ExternalId, scienceId);

        // Act
        var response = await httpClient.GetAsync($"/api/v1/profiles/professional-directions/{scienceId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var professionalDirectionsResponse = await response.Content.ReadFromJsonAsync<GetProfessionalDirectionsResponse>();
        Assert.NotNull(professionalDirectionsResponse);
        Assert.NotEmpty(professionalDirectionsResponse.ProfessionalDirections);
        Assert.Equal(1, professionalDirectionsResponse.ProfessionalDirectionsCount);
        Assert.Contains(professionalDirectionsResponse.ProfessionalDirections, pd => pd.Name == professionalDirection.Name);
        Assert.Contains(professionalDirectionsResponse.ProfessionalDirections, pd => pd.ExternalId == professionalDirection.ExternalId);
        Assert.Contains(professionalDirectionsResponse.ProfessionalDirections, pd => pd.ScienceId == scienceId);
    }

    [Fact]
    public async Task GetProfessionalDirections_ReturnsEmptyCollection_WhenThereAreNoProfessionalDirections()
    {
        // Act
        var schoolYear = 2024;
        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);
        var science = (Name: "Информатика", ExternalId: 48);
        var scienceId = await dataSeeder.SeedScience(science.Name, science.ExternalId, schoolYearId);

        // Act
        var response = await httpClient.GetAsync($"/api/v1/profiles/professional-directions/{scienceId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var professionalDirectionsResponse = await response.Content.ReadFromJsonAsync<GetProfessionalDirectionsResponse>();
        Assert.NotNull(professionalDirectionsResponse);
        Assert.NotNull(professionalDirectionsResponse.ProfessionalDirections);
        Assert.Equal(0, professionalDirectionsResponse.ProfessionalDirectionsCount);
        Assert.Empty(professionalDirectionsResponse.ProfessionalDirections);
    }

    [Fact]
    public async Task GetProfessions_ReturnsProfessions_WhenProfessionsExist()
    {
        // Arrange
        var schoolYear = 2024;
        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);
        var science = (Name: "Информатика", ExternalId: 48);
        var scienceId = await dataSeeder.SeedScience(science.Name, science.ExternalId,schoolYearId);
        var professionalDirection = (Name: "Компютърни науки", ExternalId: 481);
        var professionalDirectionId = await dataSeeder.SeedProfessionalDirection(professionalDirection.Name, professionalDirection.ExternalId, scienceId);
        var profession = (Name: "Програмист", ExternalId: 481010);
        
        await dataSeeder.SeedProfession(profession.Name, profession.ExternalId, professionalDirectionId);

        // Act
        var response = await httpClient.GetAsync($"/api/v1/profiles/professions/{professionalDirectionId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var professionsResponse = await response.Content.ReadFromJsonAsync<GetProfessionsResponse>();
        Assert.NotNull(professionsResponse);
        Assert.NotEmpty(professionsResponse.Professions);
        Assert.Equal(1, professionsResponse.ProfessionsCount);
        Assert.Contains(professionsResponse.Professions, p => p.Name == profession.Name);
        Assert.Contains(professionsResponse.Professions, p => p.ExternalId == profession.ExternalId);
        Assert.Contains(professionsResponse.Professions, p => p.ProfessionalDirectionId == professionalDirectionId);
    }

    [Fact]
    public async Task GetProfessions_ReturnsEmptyCollection_WhenThereAreNoProfessions()
    {
        // Arrange
        var schoolYear = 2024;
        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);
        var science = (Name: "Информатика", ExternalId: 48);
        var scienceId = await dataSeeder.SeedScience(science.Name, science.ExternalId, schoolYearId);
        var professionalDirection = (Name: "Компютърни науки", ExternalId: 481);
        var professionalDirectionId = await dataSeeder.SeedProfessionalDirection(professionalDirection.Name, professionalDirection.ExternalId, scienceId);
        
        // Act
        var response = await httpClient.GetAsync($"/api/v1/profiles/professions/{professionalDirectionId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var professionsResponse = await response.Content.ReadFromJsonAsync<GetProfessionsResponse>();
        Assert.NotNull(professionsResponse);
        Assert.NotNull(professionsResponse.Professions);
        Assert.Equal(0, professionsResponse.ProfessionsCount);
        Assert.Empty(professionsResponse.Professions);
    }

    [Fact]
    public async Task GetSpecialties_ReturnsProfessionalSpecialties_WhenSpecialtiesExist()
    {
        // Arrange
        var schoolYear = 2024;
        var schoolYearId = await dataSeeder.SeedSchoolYear(schoolYear);
        var science = (Name: "Информатика", ExternalId: 48);
        var scienceId = await dataSeeder.SeedScience(science.Name, science.ExternalId,schoolYearId);
        var professionalDirection = (Name: "Компютърни науки", ExternalId: 481);
        var professionalDirectionId = await dataSeeder.SeedProfessionalDirection(professionalDirection.Name, professionalDirection.ExternalId, scienceId);
        var profession = (Name: "Програмист", ExternalId: 481010);
        var professionId = await dataSeeder.SeedProfession(profession.Name, profession.ExternalId, professionalDirectionId);
        var specialty = (Name: "Програмно осигуряване", ExternalId: 4810101, IsProfessional: true);

        await dataSeeder.SeedSpecialty(specialty.Name, specialty.ExternalId, specialty.IsProfessional, professionId);

        // Act
        var response = await httpClient.GetAsync($"/api/v1/profiles/specialties/{professionId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var specialtiesResponse = await response.Content.ReadFromJsonAsync<GetSpecialtiesResponse>();
        Assert.NotNull(specialtiesResponse);
        Assert.NotEmpty(specialtiesResponse.Specialties);
        Assert.Equal(1, specialtiesResponse.SpecialtiesCount);
        Assert.Contains(specialtiesResponse.Specialties, s => s.Name == specialty.Name);
        Assert.Contains(specialtiesResponse.Specialties, s => s.ExternalId == specialty.ExternalId);
        Assert.Contains(specialtiesResponse.Specialties, s => s.IsProfessional == specialty.IsProfessional);
        Assert.Contains(specialtiesResponse.Specialties, s => s.ProfessionId == professionId);
    }

    [Fact]
    public async Task GetSpecialties_ReturnsProfiledSpecialties_WhenSpecialtiesExist()
    {
        // Arrange
        var specialty = (Name: "Икономическо развитие", ExternalId: 80101, IsProfessional: false);

        await dataSeeder.SeedSpecialty(specialty.Name, specialty.ExternalId, specialty.IsProfessional);

        // Act
        var response = await httpClient.GetAsync($"/api/v1/profiles/specialties/0");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var specialtiesResponse = await response.Content.ReadFromJsonAsync<GetSpecialtiesResponse>();
        Assert.NotNull(specialtiesResponse);
        Assert.NotEmpty(specialtiesResponse.Specialties);
        Assert.Equal(1, specialtiesResponse.SpecialtiesCount);
        Assert.Contains(specialtiesResponse.Specialties, s => s.Name == specialty.Name);
        Assert.Contains(specialtiesResponse.Specialties, s => s.ExternalId == specialty.ExternalId);
        Assert.Contains(specialtiesResponse.Specialties, s => s.IsProfessional == specialty.IsProfessional);
    }

    // TODO @IvayloK - add tests for GetExamStagesScores
}

