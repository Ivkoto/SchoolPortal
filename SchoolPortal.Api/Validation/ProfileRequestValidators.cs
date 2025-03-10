using FluentValidation;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Validation;

public class GeoLocationValidator : AbstractValidator<GeoLocationModel>
{
    public GeoLocationValidator()
    {
        RuleFor(x => x.Latitude)
            .NotNull()
            .WithMessage("Latitude value must be provided and not null.")
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees.");

        RuleFor(x => x.Longitude)
            .NotNull()
            .WithMessage("Longitude value must be provided and not null.")
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees.");

        RuleFor(x => x.Radius)
            .NotNull()
            .WithMessage("Radius value must be provided and not null.")
            .GreaterThan(-1)
            .WithMessage("Radius value cannot be a negative number.")
            .LessThan(1000)
            .WithMessage("Radius value must be no more than 999.");
    }
}

public class ProfileValidator : AbstractValidator<GetFilteredProfilesRequest>
{
    public ProfileValidator()
    {
        RuleFor(x => x.ProfileType)
            .Must(IsValidProfileType)
            .WithMessage($"Must be either '{CustomEnums.ProfileTypes.Professional}' or '{CustomEnums.ProfileTypes.Profiled}', or it can be null.")
            .WithName("ProfileType");

        RuleFor(x => x.SchoolYear)
            .SetValidator(new SchoolYearValidator());

        RuleFor(x => x.Settlement)
            .Must(settlement => settlement != null && settlement.Equals("София", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Must be София")
            .WithName("Settlement");

        RuleFor(x => x.Grade)
            .SetValidator(new GradeValidator());
    }

    public class SchoolYearValidator : AbstractValidator<int>
    {
        public SchoolYearValidator()
        {
            //TODO @IvayloK - Increase the max range of the year to 2030 for example.
            RuleFor(x => x)
                .InclusiveBetween(2010, 2030)
                .WithMessage(year => $"Provided year ({year}) must be between 2010 and 2030, inclusive.")
                .WithName("SchoolYear");
        }
    }

    public class GradeValidator : AbstractValidator<int>
    {
        public GradeValidator()
        {
            RuleFor(x => x)
                    .Must(grade => new[] { 4, 7, 10, 12 }.Contains(grade))
                    .WithMessage("Must be one of the following: 4, 7, 10, 12")
                    .WithName("Grade");
        }
    }

    private bool IsValidProfileType(string? profileType)
        => profileType is null
        || profileType.ToLower() == CustomEnums.ProfileTypes.Professional
        || profileType.ToLower() == CustomEnums.ProfileTypes.Profiled;
}
