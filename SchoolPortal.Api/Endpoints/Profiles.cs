using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;
using SchoolPortal.Api.Validation;

namespace SchoolPortal.Api.Endpoints;

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
            .Produces<GetSciencesResponse>(StatusCodes.Status200OK)
            .RequireCors("AllowedOriginsPolicy");

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
        services.TryAddScoped<IProfileRepository, ProfileRepository>();
        services.TryAddTransient<IValidator<GetFilteredProfilesRequest>, ProfileValidator>();
        services.TryAddTransient<IValidator<GeoLocationModel>, GeoLocationValidator>();
    }

    internal async Task<IResult> GetFilteredProfiles(
        HttpContext httpContext,
        [FromBody] GetFilteredProfilesRequest filters,
        [FromServices] IProfileRepository profilesRepository,
        [FromServices] IValidator<GetFilteredProfilesRequest> filtersValidator,
        [FromServices] IValidator<GeoLocationModel> locationValidator)
    {
        httpContext.Response.Headers["Deprecated"] = "False";
        //TODO @IvayloK: Enable once you deprecate endpoints
        //httpContext.Response.Headers["Deprecation-Message"] = "This API version is deprecated. Please use /api/v2/profiles/lookup.";

        var filtersValidationResult = await filtersValidator.ValidateAsync(filters);

        if (!filtersValidationResult.IsValid)
        {
            throw new ValidationException(filtersValidationResult.Errors);
        }

        if (filters.GeoLocationFilter is not null)
        {
            var locationValidationResult = await locationValidator.ValidateAsync(filters.GeoLocationFilter);
            if (!locationValidationResult.IsValid)
            {
                throw new ValidationException(locationValidationResult.Errors);
            }
        }

        var profiles = await profilesRepository.GetFilteredProfiles(filters);

        var paginationHeader = $"PageNumber={filters.PageNumber ?? 1},PageSize={filters.PageSize ?? profiles.Profiles.Count},TotalPages={profiles.TotalPages}";
        httpContext.Response.Headers["X-Pagination"] = paginationHeader;

        return Results.Ok(
            new GetFilteredProfilesResponse
            {
                ProfilesCount = profiles.Profiles.Count,
                Profiles = profiles.Profiles,
                PageNumber = filters.PageNumber ?? 1,
                PageSize = filters.PageSize ?? profiles.Profiles.Count,
                TotalPages = profiles.TotalPages
            }
        );
    }

    internal async Task<IResult> GetProfileById(
        int profileId,
        HttpContext httpContext,
        [FromServices] IProfileRepository profilesRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        var currentProfile = await profilesRepository.GetProfileById(profileId);

        return Results.Ok(currentProfile);
    }

    internal async Task<IResult> GetSciences(
        HttpContext httpContext,
        [FromServices] IProfileRepository profilesRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        var sciences = await profilesRepository.GetAllSciences();

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
        HttpContext httpContext,
        [FromServices] IProfileRepository profilesRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        var professionalDirections = await profilesRepository.GetProfessionalDirectionsByScienceId(scienceId);

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
        HttpContext httpContext,
        [FromServices] IProfileRepository profilesRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        var professions = await profilesRepository.GetProfessionsByProfessionalDirectionId(professionalDirectionId);

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
        HttpContext httpContext,
        [FromServices] IProfileRepository profilesRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        // If professionId is set to '0', it means we want to retrieve only the 'профилирани' specialties.
        // If professionId is greater than 0, it means we are requesting 'професионални' specialties.
        var specialties = await profilesRepository.GetSpecialtiesByProfessionId(professionId);

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
        HttpContext httpContext,
        [FromServices] IProfileRepository profilesRepository)
    {
        httpContext.Response.Headers["Deprecated"] = "False";

        var scores = await profilesRepository.GetAllExamStageScores(profileId, schoolYear);

        return Results.Ok(
            new GetExamStagesScoresResponse
            {
                StagesCount = scores.Count,
                ExamStageScores = scores
            }
        );
    }
}
