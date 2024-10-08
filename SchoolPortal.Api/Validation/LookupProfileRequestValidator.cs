using FluentValidation;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Validation
{
    public class GeoLocationValidator : AbstractValidator<GeoLocationRequest>
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

    public class LookupProfilesValidator : AbstractValidator<LookupProfilesRequest> 
    {
        public LookupProfilesValidator()
        {
            RuleFor(x => x.ProfileType)
                .Must(IsValidProfileType)
                .WithMessage($"Provided profile type must be either '{CustomEnums.ProfileType.Professional}' or '{CustomEnums.ProfileType.Profiled}'.");
        }

        private bool IsValidProfileType(string? profileType)
            => profileType?.ToLower() == CustomEnums.ProfileType.Professional 
            || profileType?.ToLower() == CustomEnums.ProfileType.Profiled;
    }
}
