using FluentValidation;

namespace SchoolPortal.Api.Validation
{
    public class InstitutionIdValidator : AbstractValidator<int>
    {
        public InstitutionIdValidator()
        {
            RuleFor(institutionId => institutionId)
                .NotNull()
                .WithMessage("Institution ID cannot be NULL.")
                .GreaterThan(0)
                .WithMessage("Institution ID must be a positive number.");
        }
    }
}
