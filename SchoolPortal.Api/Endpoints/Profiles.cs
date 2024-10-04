using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

            //Results from Mock Data
            app.MapGet("/profiles/specialties", GetSpecialties);
        }
            
        internal async Task<IResult> GetFilteredProfiles(
            [FromBody] LookupProfilesRequest filters, [FromServices] IProfileRepository service,
            IValidator<GeoLocationRequest> validator, CancellationToken cancellationToken)
        {
            if (filters.GeoLocationFilter is not null)
            {
                var validationResult = await validator.ValidateAsync(filters.GeoLocationFilter, cancellationToken);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
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


        internal async Task<IEnumerable<SpecialtyModel>> GetSpecialties()
        {
            string filePath = "MockData/Specialties.json";
            string fileContent;

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
            }
            var root = JsonConvert.DeserializeObject<SpecialtiesRoot>(fileContent);

            return root?.Specialties ?? new List<SpecialtyModel>();
        }

        private async Task<List<string>> ReadFromFileAsync(string filePath)
        {
            string fileContent;

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
            }

            var professionalDirections = JsonConvert.DeserializeObject<List<string>>(fileContent);

            return professionalDirections ?? new List<string>();
        }

        public void MapServices(IServiceCollection services)
        {
            services.AddSingleton<IProfileRepository, ProfileRepository>();
            services.AddScoped<IValidator<GeoLocationRequest>, GeoLocationValidator>();
        }
    }
}
