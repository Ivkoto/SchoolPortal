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
               .Produces<GetFilteredProfilesResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/{profileId:int}", GetProfileById)
                .WithName("GetProfileById")
                .Produces<ProfileModel>(StatusCodes.Status200OK);

            app.MapGet("/profiles/sciences", GetSciences)
                .WithName("GetSciences")
                .Produces<GetSciencesResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/professional-directions/{scienceId:int}", GetProfessionalDirections)
                .WithName("GetProfessionalDirections")
                .Produces<GetProfessionalDirectionsResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/professions/{ProfessionalDirectionId:int}", GetProfessions)
                .WithName("GetProfessions")
                .Produces<GetProfessionsResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/specialties/{professionId:int}", GetSpecialties)
                .WithName("GetSpecialties")
                .Produces<GetSpecialtiesResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/{profileId:int}/exam-stages/{schoolYear:int}", GetExamStagesScores)
                .WithName("GetExamStagesScores")
                .Produces<GetExamStagesScoresResponse>(StatusCodes.Status200OK);
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddTransient<IValidator<GetFilteredProfilesRequest>, ProfileValidator>();
            services.AddTransient<IValidator<GeoLocationModel>, GeoLocationValidator>();            
        }

        internal async Task<IResult> GetFilteredProfiles(
            [FromBody] GetFilteredProfilesRequest filters,
            [FromServices] IProfileRepository service,
            [FromServices] IValidator<GetFilteredProfilesRequest> filtersValidator,
            [FromServices] IValidator<GeoLocationModel> locationValidator)
        {
            var filtersValidationResult = await filtersValidator.ValidateAsync(filters);
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

            var profiles = await service.GetFilteredProfiles(filters);

            return Results.Ok(
                new GetFilteredProfilesResponse
                {
                    ProfilesCount = profiles.Count,
                    Profiles = profiles
                }
            );
        }

        internal async Task<IResult> GetProfileById(
            int profileId,
            [FromServices] IProfileRepository service)
        {
            var currentProfile = await service.GetProfileById(profileId);
            
            return Results.Ok(currentProfile);
        }

        internal async Task<IResult> GetSciences(
            [FromServices] IProfileRepository service)
        {
            var sciences = await service.GetAllSciences();

            return Results.Ok(
                new GetSciencesResponse
                {
                    SciencesCount = sciences.Count,
                    Sciences = sciences
                }
            );
        }

        internal async Task<IResult> GetProfessionalDirections(
            int scienceId,
            [FromServices] IProfileRepository service)
        {
            var professionalDirections = await service.GetProfessionalDirectionsByScienceId(scienceId);

            return Results.Ok(
                new GetProfessionalDirectionsResponse
                {
                    ProfessionalDirectionsCount = professionalDirections.Count,
                    ProfessionalDirections = professionalDirections
                }
            );
        }

        internal async Task<IResult> GetProfessions(
            int professionalDirectionId,
            [FromServices] IProfileRepository service)
        {
            var professions = await service.GetProfessionsByProfessionalDirectionId(professionalDirectionId);

            return Results.Ok(
                new GetProfessionsResponse
                {
                    ProfessionsCount = professions.Count,
                    Professions = professions
                }
            );
        }

        internal async Task<IResult> GetSpecialties(
            int professionId,
            [FromServices] IProfileRepository service)
        {
            var specialties = await service.GetSpecialtiesByProfessionId(professionId);

            return Results.Ok(
                new GetSpecialtiesResponse
                {
                    SpecialtesCount = specialties.Count,
                    Specialties = specialties
                }
            );
        }

        internal async Task<IResult> GetExamStagesScores(
            int profileId,
            int schoolYear,
            [FromServices] IProfileRepository service)
        {
            var scores = await service.GetAllExamStageScores(profileId, schoolYear);

            return Results.Ok(
                new GetExamStagesScoresResponse
                {
                    StagesCount = scores.Count,
                    ExamStageScores = scores
                }
            );
        }
    }
}
