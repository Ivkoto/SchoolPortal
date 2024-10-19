using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;
using SchoolPortal.Api.Validation;

namespace SchoolPortal.Api.Endpoints
{
    public class Institutions : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/institutions/{institutionId:int}", GetInstitutionById)
                .WithName("GetInstitutionById")
                .Produces<InstitutionModel>(StatusCodes.Status200OK);

            app.MapGet("/institutions/{institutionId:int}/profiles", GetInstitutionProfiles)
                .WithName("GetInstitutionProfiles")
                .Produces<LookupProfilesResponse>(StatusCodes.Status200OK);
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddSingleton<IInstitutionRepository, InstitutionRepository>();
            services.AddScoped<IValidator<int>, InstitutionIdValidator>();
        }

        internal async Task<IResult> GetInstitutionById(
            int institutionId,
            [FromServices] IInstitutionRepository service,
            [FromServices] IValidator<int> institutionsIdValidator)
        {
            var validationResult = await institutionsIdValidator.ValidateAsync(institutionId);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var currentInstitution = await service.GetInstitutionAsync(institutionId);

            return Results.Ok(currentInstitution);
        }

        internal async Task<IResult> GetInstitutionProfiles(
            int institutionId,
            [FromQuery] int schoolYear,
            [FromQuery] int? grade,
            [FromServices] IInstitutionRepository service,
            [FromServices] IValidator<int> institutionsIdValidator)
        {
            var validationResult = await institutionsIdValidator.ValidateAsync(institutionId);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var profiles = await service.GetInstitutionProfiles(institutionId, schoolYear, grade);

            return Results.Ok(new LookupProfilesResponse
            {
                ProfileCount = profiles.Count,
                Profiles = profiles
            });
        }
    }
}
