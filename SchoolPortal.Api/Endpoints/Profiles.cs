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
               .WithName("GetProfiles")
               .Produces<LookupProfilesResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/sciences", GetSciences)
                .WithName("GetSciences")
                .Produces<LookupSciencesResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/professional-directions/{scienceId:int}", GetProfessionalDirections)
                .WithName("GetProfessionalDirections")
                .Produces<LookupProfessionalDirectionsResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/professions/{ProfessionalDirectionId:int}", GetProfessions)
                .WithName("GetProfessions")
                .Produces<LookupProfessionsResponse>(StatusCodes.Status200OK);

            app.MapGet("/profiles/specialties/{profileType}/{professionId:int?}", GetSpecialties)
                .WithName("GetSpecialties")
                .Produces<LookupSpecialtyResponse>(StatusCodes.Status200OK);
        }

        internal async Task<IResult> GetFilteredProfiles(
            [FromBody] LookupProfilesRequest filters, [FromServices] IProfileRepository service,
            IValidator<LookupProfilesRequest> filtersValidator, IValidator<GeoLocationRequest> locationValidator,
            CancellationToken cancellationToken)
        {
            if (filters.GeoLocationFilter is not null)
            {
                var locationValidationResult = await locationValidator.ValidateAsync(filters.GeoLocationFilter, cancellationToken);
                if (!locationValidationResult.IsValid)
                    return Results.ValidationProblem(locationValidationResult.ToDictionary());
            }

            if (filters.ProfileType is not null)
            {
                var profileValidationResult = await filtersValidator.ValidateAsync(filters, cancellationToken);
                if (!profileValidationResult.IsValid)
                    return Results.ValidationProblem(profileValidationResult.ToDictionary());
            } 

            var profiles = await service.GetFilteredProfiles(filters, cancellationToken);

            return Results.Ok(new LookupProfilesResponse
            {
                ProfileCount = profiles.Count,
                Profiles = profiles
            });
        }

        internal async Task<IResult> GetSciences(
            [FromServices] IProfileRepository service, CancellationToken cancellationToken)
        {
            var sciences = await service.GetAllSciences(cancellationToken);

            return Results.Ok(new LookupSciencesResponse
            {
                ScienceCount = sciences.Count,
                Sciences = sciences
            });
        }

        internal async Task<IResult> GetProfessionalDirections(
            int scienceId,
            [FromServices] IProfileRepository service,
            CancellationToken cancellationToken)
        {
            var professionalDirections = await service.GetProfessionalDirectionsByScienceId(scienceId, cancellationToken);

            return Results.Ok(new LookupProfessionalDirectionsResponse
            {
                ProfessionalDirectionCount = professionalDirections.Count,
                ProfessionalDirections = professionalDirections
            });
        }

        internal async Task<IResult> GetProfessions(
            int professionalDirectionId,
            [FromServices] IProfileRepository service,
            CancellationToken cancellationToken)
        {
            var professions = await service.GetProfessionsByProfessionalDirectionId(professionalDirectionId, cancellationToken);

            return Results.Ok(new LookupProfessionsResponse
            {
                ProfessionCount = professions.Count,
                Professions = professions
            });
        }


        internal async Task<IResult> GetSpecialties(
            string profileType,
            int? professionId,
            [FromServices] IProfileRepository service,
            CancellationToken cancellationToken)
        {
            var isProfiled = profileType == CustomEnums.ProfileType.Profiled;
            var isProfessional = profileType == CustomEnums.ProfileType.Professional;

            if (!isProfessional && !isProfiled)
            {
                return Results.BadRequest($"Invalid value for 'Profile Type'. It must be either '{CustomEnums.ProfileType.Professional}' or '{CustomEnums.ProfileType.Profiled}'");
            }
            if (isProfiled && professionId.HasValue)
            {
                return Results.BadRequest($"If the specialty is not of type {CustomEnums.ProfileType.Professional}, it does not contain a profession ID.");
            }

            var specialties = await service.GetSpecialtiesByProfessionId(profileType, cancellationToken, professionId);

            return Results.Ok(new LookupSpecialtyResponse
            {
                SpecialtyCount = specialties.Count,
                Specialties = specialties
            });
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddSingleton<IProfileRepository, ProfileRepository>();
            services.AddScoped<IValidator<LookupProfilesRequest>, LookupProfilesValidator>();
            services.AddScoped<IValidator<GeoLocationRequest>, GeoLocationValidator>();
        }
    }
}
