using FluentValidation.TestHelper;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Validation;

namespace SchoolPortal.UnitTests.Validation;

public class ProfileRequestValidatorsTests
{
    private readonly ProfileValidator profileValidator;
    private readonly ProfileValidator.SchoolYearValidator schoolYearValidator;
    private readonly ProfileValidator.GradeValidator gradeValidator;
    private readonly GeoLocationValidator geoLocationValidator;

    public ProfileRequestValidatorsTests()
    {
        profileValidator = new ProfileValidator();
        schoolYearValidator = new ProfileValidator.SchoolYearValidator();
        gradeValidator = new ProfileValidator.GradeValidator();
        geoLocationValidator = new GeoLocationValidator();
    }

    [Fact]
    public void ProfileValidator_ShouldHaveError_When_ProfileTypeIsInvalid()
    {
        // Arrange
        var model = new GetFilteredProfilesRequest
            (SchoolYear: 2024, Grade: 7, Settlement: "София", null, null, ProfileType: "InvalidType", null, null, null, null, null, null);

        // Act
        var result = profileValidator.TestValidate(model);

        // Assert
        Assert.False(result.IsValid);
        var erorr = Assert.Single(result.Errors);
        Assert.Equal("ProfileType", erorr.PropertyName);
        Assert.Equal("Must be either 'професионална' or 'профилирана', or it can be null.", erorr.ErrorMessage);
    }

    [Fact]
    public void ProfileValidator_ShouldNotHaveError_When_ProfileTypeIsValid()
    {
        // Arrange
        var model = new GetFilteredProfilesRequest
            (SchoolYear: 2024, Grade: 7, Settlement:"София", null, null, ProfileType: CustomEnums.ProfileTypes.Professional, null, null, null, null, null, null);

        // Act
        var result = profileValidator.TestValidate(model);


        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ProfileValidator_ShouldNotHaveError_When_SettlementIsValid()
    {
        //Arrange
        var model = new GetFilteredProfilesRequest
            (SchoolYear: 2024, Grade: 7, Settlement: "София", null, null, CustomEnums.ProfileTypes.Profiled, null, null, null, null, null, null);

        var result = profileValidator.TestValidate(model);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ProfileValidator_ShouldHaveError_When_SettlementIsInvalid()
    {
        //Arrange
        var model = new GetFilteredProfilesRequest
            (SchoolYear: 2024, Grade: 7, Settlement: "InvalidSettlement", null, null, CustomEnums.ProfileTypes.Profiled, null, null, null, null, null, null);

        var result = profileValidator.TestValidate(model);

        //Assert
        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal("Settlement", error.PropertyName);
        Assert.Equal("Must be София", error.ErrorMessage);
    }

    [Fact]
    public void SchoolYearValidator_ShouldHaveErrorWhen_SchoolYearIsOutOfRange()
    {
        // Arrange
        int invalidSchoolYear = 2009;

        // Act
        var result = schoolYearValidator.TestValidate(invalidSchoolYear);

        // Assert
        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal($"Provided year ({invalidSchoolYear}) must be between 2010 and 2030, inclusive.", error.ErrorMessage);
    }

    [Fact]
    public void SchoolYearValidator_ShouldNotHaveError_WhenSchoolYearIsInRange()
    {
        // Arrange
        int validSchoolYear = 2024;

        //Act
        var result = schoolYearValidator.TestValidate(validSchoolYear);

        //Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void GradeValidator_ShouldHaveError_WhenGradeIsInvalid()
    {
        // Arrange
        int invalidGrade = 5;

        // Act
        var result = gradeValidator.TestValidate(invalidGrade);

        // Assert
        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal("Must be one of the following: 4, 7, 10, 12", error.ErrorMessage);
    }

    [Fact]
    public void GradeValidator_ShouldNotHaveError_WhenGradeIsValid()
    {
        // Arrange
        int validGrade = 7;

        // Act
        var result = gradeValidator.TestValidate(validGrade);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void GeoLocationValidator_ShouldHaveError_WhenGeoLocationIsInvalid()
    {
        // Arrange
        var model = new GeoLocationModel(100, 200, -1);

        // Act
        var result = geoLocationValidator.TestValidate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(3, result.Errors.Count);

        var latitudeError = result.Errors.FirstOrDefault(e => e.PropertyName == "Latitude");
        var longitudeError = result.Errors.FirstOrDefault(e => e.PropertyName == "Longitude");
        var radiusError = result.Errors.FirstOrDefault(e => e.PropertyName == "Radius");

        Assert.NotNull(latitudeError);
        Assert.Equal("Latitude must be between -90 and 90 degrees.", latitudeError.ErrorMessage);

        Assert.NotNull(longitudeError);
        Assert.Equal("Longitude must be between -180 and 180 degrees.", longitudeError.ErrorMessage);

        Assert.NotNull(radiusError);
        Assert.Equal("Radius value cannot be a negative number.", radiusError.ErrorMessage);
    }

    [Fact]
    public void GeoLocationValidator_ShouldNotHaveError_WhenGeoLocationIsValid()
    {
        // Arrange
        var model = new GeoLocationModel(42.0m, 23.0m, 10);

        var result = geoLocationValidator.TestValidate(model);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}

