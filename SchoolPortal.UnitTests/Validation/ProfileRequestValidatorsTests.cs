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
        var model = new GetFilteredProfilesRequest
            (SchoolYear: 2024, Grade: 7, null, null, null, ProfileType: "InvalidType", null, null, null, null, null, null);

        var result = profileValidator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.ProfileType)
              .WithErrorMessage($"Provided profile type must be either '{CustomEnums.ProfileTypes.Professional}' or '{CustomEnums.ProfileTypes.Profiled}', or it can be null.");
    }

    [Fact]
    public void ProfileValidator_ShouldNotHaveError_When_ProfileTypeIsValid()
    {
        var model = new GetFilteredProfilesRequest
            (SchoolYear: 2024, Grade: 7, null, null, null, ProfileType: CustomEnums.ProfileTypes.Professional, null, null, null, null, null, null);

        var result = profileValidator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.ProfileType);
    }

    [Fact]
    public void ProfileValidator_ShouldNotHaveError_When_SettlementIsValid()
    {
        var model = new GetFilteredProfilesRequest
            (SchoolYear: 2024, Grade: 7, Settlement: "София", null, null, CustomEnums.ProfileTypes.Profiled, null, null, null, null, null, null);

        var result = profileValidator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Settlement);
    }

    [Fact]
    public void ProfileValidator_ShouldHaveError_When_SettlementIsInvalid()
    {
        var model = new GetFilteredProfilesRequest
            (SchoolYear: 2024, Grade: 7, Settlement: "InvalidSettlement", null, null, CustomEnums.ProfileTypes.Profiled, null, null, null, null, null, null);

        var result = profileValidator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Settlement)
              .WithErrorMessage("Settlement must be София");
    }

    [Fact]
    public void SchoolYearValidator_ShouldHaveErrorWhen_SchoolYearIsOutOfRange()
    {
        var result = schoolYearValidator.TestValidate(2009);
        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("SchoolYear must be between 2010 and 2024, inclusive.");
    }

    [Fact]
    public void SchoolYearValidator_ShouldNotHaveError_WhenSchoolYearIsInRange()
    {
        var result = schoolYearValidator.TestValidate(2024);

        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void GradeValidator_ShouldHaveError_WhenGradeIsInvalid()
    {
        var result = gradeValidator.TestValidate(5);

        result.ShouldHaveValidationErrorFor(x => x)
            .WithErrorMessage("Grade must be one of the following: 4, 7, 10, 12");
    }

    [Fact]
    public void GradeValidator_ShouldNotHaveError_WhenGradeIsValid()
    {
        var result = gradeValidator.TestValidate(7);

        result.ShouldNotHaveValidationErrorFor(x => x);
    }

    [Fact]
    public void GeoLocationValidator_ShouldHaveError_WhenGeoLocationIsInvalid()
    {
        var model = new GeoLocationModel(100, 200, -1);

        var result = geoLocationValidator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(x => x.Latitude)
            .WithErrorMessage("Latitude must be between -90 and 90 degrees.");
        result.ShouldHaveValidationErrorFor(x => x.Longitude)
            .WithErrorMessage("Longitude must be between -180 and 180 degrees.");
        result.ShouldHaveValidationErrorFor(x => x.Radius)
            .WithErrorMessage("Radius value cannot be a negative number.");
    }

    [Fact]
    public void GeoLocationValidator_ShouldNotHaveError_WhenGeoLocationIsValid()
    {
        var model = new GeoLocationModel(42.0m, 23.0m, 10);

        var result = geoLocationValidator.TestValidate(model);

        result.ShouldNotHaveValidationErrorFor(x => x.Latitude);
        result.ShouldNotHaveValidationErrorFor(x => x.Longitude);
        result.ShouldNotHaveValidationErrorFor(x => x.Radius);
    }
}

