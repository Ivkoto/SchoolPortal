//using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;

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
                .Produces<GetFilteredProfilesResponse>(StatusCodes.Status200OK);
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddScoped<IInstitutionRepository, InstitutionRepository>();
        }

        internal async Task<IResult> GetInstitutionById(
            int institutionId,
            [FromServices] IInstitutionRepository service)
        {
            var currentInstitution = await service.GetInstitutionAsync(institutionId);

            return Results.Ok(currentInstitution);
        }

        internal async Task<IResult> GetInstitutionProfiles(
            int institutionId,
            [FromQuery] int schoolYear,
            [FromQuery] int? grade,
            [FromServices] IInstitutionRepository service)
        {
            var profiles = await service.GetInstitutionProfiles(institutionId, schoolYear, grade);

            return Results.Ok(
                new GetFilteredProfilesResponse
                {
                    ProfilesCount = profiles.Count,
                    Profiles = profiles
                }
            );
        }
    }
}
