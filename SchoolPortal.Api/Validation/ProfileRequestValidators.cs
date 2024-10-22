using FluentValidation;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Validation
{
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
                .WithMessage($"Provided profile type must be either '{CustomEnums.ProfileTypes.Professional}' or '{CustomEnums.ProfileTypes.Profiled}', or it can be null.");

            RuleFor(x => x.SchoolYear)
                .InclusiveBetween(2024, 2024)
                .WithMessage("SchoolYear must be 2024.");

            RuleFor(x => x.Settlement)
                .Must(settlement => settlement != null && settlement.Equals("София", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Settlement must be София");

            RuleFor(x => x.Grade)
                .Must(grade => new[] { 5, 8, 11 }.Contains(grade))
                .WithMessage("Grade must be one of the following: 5, 8, 11");
        }

        private bool IsValidProfileType(string? profileType)
            => profileType is null
            || profileType.ToLower() == CustomEnums.ProfileTypes.Professional
            || profileType.ToLower() == CustomEnums.ProfileTypes.Profiled;
    }
}
