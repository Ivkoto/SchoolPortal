using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;

namespace SchoolPortal.Api.Endpoints
{
    public class Institutions : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/api/v1/institutions/{institutionId:int}", GetInstitutionById)
                .WithName("GetInstitutionById")
                .Produces<InstitutionModel>(StatusCodes.Status200OK);

            app.MapGet("/api/v1/institutions/{institutionId:int}/profiles", GetInstitutionProfiles)
                .WithName("GetInstitutionProfiles")
                .Produces<GetFilteredProfilesResponse>(StatusCodes.Status200OK);

            app.MapGet("/api/v1/institutions/{institutionId:int}/average-successes", GetInstitutionAverageSuccesses)
                .WithName("GetInstitutionAverageSuccesses")
                .Produces<GetFilteredProfilesResponse>(StatusCodes.Status200OK);
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddScoped<IInstitutionRepository, InstitutionRepository>();
        }

        internal async Task<IResult> GetInstitutionById(
            int institutionId,
            HttpContext httpContext,
            [FromServices] IInstitutionRepository service)
        {
            httpContext.Response.Headers["Deprecated"] = "False";

            var currentInstitution = await service.GetInstitutionById(institutionId);

            return Results.Ok(currentInstitution);
        }

        internal async Task<IResult> GetInstitutionProfiles(
            int institutionId,
            HttpContext httpContext,
            [FromQuery] int schoolYear,
            [FromQuery] int? grade,
            [FromServices] IInstitutionRepository service)
        {
            httpContext.Response.Headers["Deprecated"] = "False";

            var profiles = await service.GetInstitutionProfiles(institutionId, schoolYear, grade);

            return Results.Ok(
                new GetFilteredProfilesResponse
                {
                    ProfilesCount = profiles.Count,
                    Profiles = profiles
                }
            );
        }

        internal async Task<IResult> GetInstitutionAverageSuccesses(
            int institutionId,
            HttpContext httpContext,
            [FromQuery] int schoolYear,
            [FromQuery] int grade,
            [FromServices] IInstitutionRepository service)
        {
            httpContext.Response.Headers["Deprecated"] = "False";

            var examResults = await service.GetInstitutionAverageSuccesses(institutionId, schoolYear, grade);

            return Results.Ok(
                new GetExamResultsResponse
                {
                    ExamResultsCount = examResults.Count,
                    ExamResults = examResults
                }
            );
        }
    }
}
