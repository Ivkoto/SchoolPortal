using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;
using SchoolPortal.Api.Validation;

namespace SchoolPortal.Api.Endpoints
{
    public class Profiles : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapPost("/profiles/lookup", GetFilteredProfiles)
               .WithName("GetFilteredProfiles")
               .Produces<LookupProfilesResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/{profileId:int}", GetProfileById)
                .WithName("GetProfileById")
                .Produces<ProfileModel>(StatusCodes.Status200OK);

            app.MapGet("/profiles/sciences", GetSciences)
                .WithName("GetSciences")
                .Produces<LookupSciencesResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/professional-directions/{scienceId:int}", GetProfessionalDirections)
                .WithName("GetProfessionalDirections")
                .Produces<LookupProfessionalDirectionsResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/professions/{ProfessionalDirectionId:int}", GetProfessions)
                .WithName("GetProfessions")
                .Produces<LookupProfessionsResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/specialties/{professionId:int}", GetSpecialties)
                .WithName("GetSpecialties")
                .Produces<LookupSpecialtyResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/{profileId:int}/exam-stages/{schoolYear:int}", GetExamStagesScores)
                .WithName("GetExamStagesScores")
                .Produces<ExamStageScoresResponse>(StatusCodes.Status200OK);
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddSingleton<IProfileRepository, ProfileRepository>();
            services.AddScoped<IValidator<LookupProfilesRequest>, LookupProfilesValidator>();
            services.AddScoped<IValidator<GeoLocationRequest>, GeoLocationValidator>();
            services.AddScoped<IValidator<(int profileId, int schoolYear)>, ProfileExamStageValidator>();
        }

        internal async Task<IResult> GetFilteredProfiles(
            [FromBody] LookupProfilesRequest filters,
            [FromServices] IProfileRepository service,
            [FromServices] IValidator<LookupProfilesRequest> filtersValidator,
            [FromServices] IValidator<GeoLocationRequest> locationValidator)
        {
            var filtersValidationResult = filtersValidator.Validate(filters);
            if (!filtersValidationResult.IsValid)
            {
                return Results.ValidationProblem(filtersValidationResult.ToDictionary());
            }

            if (filters.GeoLocationFilter is not null)
            {
                var locationValidationResult = await locationValidator.ValidateAsync(filters.GeoLocationFilter);
                if (!locationValidationResult.IsValid)
                {
                    return Results.ValidationProblem(locationValidationResult.ToDictionary());
                }
            }

            if (filters.ProfileType is not null)
            {
                var profileValidationResult = await filtersValidator.ValidateAsync(filters);
                if (!profileValidationResult.IsValid)
                {
                    return Results.ValidationProblem(profileValidationResult.ToDictionary());
                }
            }

            var profiles = await service.GetFilteredProfiles(filters);

            return Results.Ok(new LookupProfilesResponse
            {
                ProfileCount = profiles.Count,
                Profiles = profiles
            });
        }

        internal async Task<IResult> GetProfileById(
            int profileId, [FromServices] IProfileRepository service)
        {
            var currentProfile = await service.GetProfileById(profileId);

            return Results.Ok(currentProfile);
        }

        internal async Task<IResult> GetSciences(
            [FromServices] IProfileRepository service)
        {
            var sciences = await service.GetAllSciences();

            return Results.Ok(new LookupSciencesResponse
            {
                ScienceCount = sciences.Count,
                Sciences = sciences
            });
        }

        internal async Task<IResult> GetProfessionalDirections(
            int scienceId, [FromServices] IProfileRepository service)
        {
            var professionalDirections = await service.GetProfessionalDirectionsByScienceId(scienceId);

            return Results.Ok(new LookupProfessionalDirectionsResponse
            {
                ProfessionalDirectionCount = professionalDirections.Count,
                ProfessionalDirections = professionalDirections
            });
        }

        internal async Task<IResult> GetProfessions(
            int professionalDirectionId, [FromServices] IProfileRepository service)
        {
            var professions = await service.GetProfessionsByProfessionalDirectionId(professionalDirectionId);

            return Results.Ok(new LookupProfessionsResponse
            {
                ProfessionCount = professions.Count,
                Professions = professions
            });
        }

        internal async Task<IResult> GetSpecialties(
            int professionId, [FromServices] IProfileRepository service)
        {
            var specialties = await service.GetSpecialtiesByProfessionId(professionId);

            return Results.Ok(new LookupSpecialtyResponse
            {
                SpecialtyCount = specialties.Count,
                Specialties = specialties
            });
        }

        internal async Task<IResult> GetExamStagesScores(
            int profileId, int schoolYear,
            [FromServices] IProfileRepository service,
            [FromServices] IValidator<(int profileId, int schoolYear)> validator)
        {
            var validationResult = validator.Validate((profileId, schoolYear));
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var scores = await service.GetAllExamStageScores(profileId, schoolYear);

            return Results.Ok(new ExamStageScoresResponse{
                StagesCount = scores.Count,
                ExamStageScores = scores
            });
        }
    }
}
