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
            app.MapPost("/api/v1/profiles/lookup", GetFilteredProfiles)
               .WithName("GetFilteredProfilesV1")
               .Produces<GetFilteredProfilesResponse>(StatusCodes.Status200OK)
               .RequireCors("PaginationPolicy");

            app.MapGet("/api/v1/profiles/{profileId:int}", GetProfileById)
                .WithName("GetProfileByIdV1")
                .Produces<ProfileModel>(StatusCodes.Status200OK)
                .RequireCors("AllowedOriginsPolicy");

            app.MapGet("/api/v1/profiles/sciences", GetSciences)
                .WithName("GetSciencesV1")
                .Produces<GetSciencesResponse>(StatusCodes.Status200OK);

            app.MapGet("/api/v1/profiles/professional-directions/{scienceId:int}", GetProfessionalDirections)
                .WithName("GetProfessionalDirectionsV1")
                .Produces<GetProfessionalDirectionsResponse>(StatusCodes.Status200OK)
                .RequireCors("AllowedOriginsPolicy");

            app.MapGet("/api/v1/profiles/professions/{ProfessionalDirectionId:int}", GetProfessions)
                .WithName("GetProfessionsV1")
                .Produces<GetProfessionsResponse>(StatusCodes.Status200OK)
                .RequireCors("AllowedOriginsPolicy");

            app.MapGet("/api/v1/profiles/specialties/{professionId:int}", GetSpecialties)
                .WithName("GetSpecialtiesV1")
                .Produces<GetSpecialtiesResponse>(StatusCodes.Status200OK)
                .RequireCors("AllowedOriginsPolicy");

            app.MapGet("/api/v1/profiles/{profileId:int}/exam-stages/{schoolYear:int}", GetExamStagesScores)
                .WithName("GetExamStagesScoresV1")
                .Produces<GetExamStagesScoresResponse>(StatusCodes.Status200OK)
                .RequireCors("AllowedOriginsPolicy");
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddTransient<IValidator<GetFilteredProfilesRequest>, ProfileValidator>();
            services.AddTransient<IValidator<GeoLocationModel>, GeoLocationValidator>();  
        }

        public async Task<IResult> GetFilteredProfiles(
            HttpContext httpContext,
            [FromBody] GetFilteredProfilesRequest filters,
            [FromServices] IProfileRepository service,
            [FromServices] IValidator<GetFilteredProfilesRequest> filtersValidator,
            [FromServices] IValidator<GeoLocationModel> locationValidator)
        {
            httpContext.Response.Headers["Deprecated"] = "False";
            //TODO @IvayloK: Enable once you deprecate endpoints
            //httpContext.Response.Headers["Deprecation-Message"] = "This API version is deprecated. Please use /api/v2/profiles/lookup.";

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

            var result = await service.GetFilteredProfiles(filters);

            var paginationHeader = $"PageNumber={filters.PageNumber ?? 1},PageSize={filters.PageSize ?? result.Profiles.Count},TotalPages={result.TotalPages}";
            httpContext.Response.Headers["X-Pagination"] = paginationHeader;

            return Results.Ok(
                new GetFilteredProfilesResponse
                {
                    ProfilesCount = result.Profiles.Count,
                    Profiles = result.Profiles,
                    PageNumber = filters.PageNumber ?? 1,
                    PageSize = filters.PageSize ?? result.Profiles.Count,
                    TotalPages = result.TotalPages
                }
            );
        }

        public async Task<IResult> GetProfileById(
            int profileId,
            HttpContext httpContext,
            [FromServices] IProfileRepository service)
        {
            httpContext.Response.Headers["Deprecated"] = "False";

            var currentProfile = await service.GetProfileById(profileId);
            
            return Results.Ok(currentProfile);
        }

        public async Task<IResult> GetSciences(
            HttpContext httpContext,
            [FromServices] IProfileRepository service)
        {
            httpContext.Response.Headers["Deprecated"] = "False";

            var sciences = await service.GetAllSciences();

            return Results.Ok(
                new GetSciencesResponse
                {
                    SciencesCount = sciences.Count,
                    Sciences = sciences
                }
            );
        }

        public async Task<IResult> GetProfessionalDirections(
            int scienceId,
            HttpContext httpContext,
            [FromServices] IProfileRepository service)
        {
            httpContext.Response.Headers["Deprecated"] = "False";

            var professionalDirections = await service.GetProfessionalDirectionsByScienceId(scienceId);

            return Results.Ok(
                new GetProfessionalDirectionsResponse
                {
                    ProfessionalDirectionsCount = professionalDirections.Count,
                    ProfessionalDirections = professionalDirections
                }
            );
        }

        public async Task<IResult> GetProfessions(
            int professionalDirectionId,
            HttpContext httpContext,
            [FromServices] IProfileRepository service)
        {
            httpContext.Response.Headers["Deprecated"] = "False";

            var professions = await service.GetProfessionsByProfessionalDirectionId(professionalDirectionId);

            return Results.Ok(
                new GetProfessionsResponse
                {
                    ProfessionsCount = professions.Count,
                    Professions = professions
                }
            );
        }

        public async Task<IResult> GetSpecialties(
            int professionId,
            HttpContext httpContext,
            [FromServices] IProfileRepository service)
        {
            httpContext.Response.Headers["Deprecated"] = "False";

            var specialties = await service.GetSpecialtiesByProfessionId(professionId);

            return Results.Ok(
                new GetSpecialtiesResponse
                {
                    SpecialtesCount = specialties.Count,
                    Specialties = specialties
                }
            );
        }

        public async Task<IResult> GetExamStagesScores(
            int profileId,
            int schoolYear,
            HttpContext httpContext,
            [FromServices] IProfileRepository service)
        {
            httpContext.Response.Headers["Deprecated"] = "False";

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
