using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SchoolPortal.Api.Models;
using SchoolPortal.Api.Repositories;

namespace SchoolPortal.Api.Endpoints
{
    public class ProfileEndpoints : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/Profiles/SearchProfiles", GetFilteredProfiles)
               .WithName("SearchProfiles")
               .Produces<IEnumerable<ProfileModel>>(StatusCodes.Status200OK);

            //Result from Mock Data
            app.MapGet("/Profiles/Filters/SearchGrades", GetGrades);
            app.MapGet("/Profiles/Filters/SearchProfessionalDirections", GetProfessionalDirections);
            app.MapGet("/Profiles/Filters/SearchProfessions", GetProfessions);
            app.MapGet("/Profiles/Filters/SearchProfileTypes", GetProfileTypes);
            app.MapGet("/Profiles/Filters/SearchSciences", GetSciences);
            app.MapGet("/Profiles/Filters/SearchSpecialties", GetSpecialties);
        }

        internal async Task<IResult> GetFilteredProfiles(
            [FromQuery] int? SchoolYear,
            [FromQuery] int? Grade,
            [FromQuery] int? SpecialtyId,
            [FromQuery] int? ProfessionId,
            [FromQuery] int? ProfessionalDirectionId,
            [FromQuery] int? ScienceId,
            [FromServices] IProfileRepository service,
            CancellationToken cancellationToken)
        {
            var filters = new ProfileFilterModel
            {
                SchoolYear = SchoolYear,
                Grade = Grade,
                SpecialtyId = SpecialtyId,
                ProfessionId = ProfessionId,
                ProfessionalDirectionId = ProfessionalDirectionId,
                ScienceId = ScienceId
            };

            var profiles = await service.GetFilteredProfiles(filters, cancellationToken);

            return Results.Ok(profiles);
        }

        internal async Task<IEnumerable<string>> GetGrades()
        {
            string filePath = $"MockData/Grade.json";
            return await ReadFromFileAsync(filePath);
        }

        internal async Task<IEnumerable<ProfessionalDirectionModel>> GetProfessionalDirections()
        {
            string filePath = "MockData/ProfessionalDirections.json";
            string fileContent;

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
            }
            var root = JsonConvert.DeserializeObject<ProfessionalDirectionsRoot>(fileContent);

            return root?.ProfessionalDirections ?? new List<ProfessionalDirectionModel>();
        }

        internal async Task<IEnumerable<ProfessionModel>> GetProfessions()
        {
            string filePath = "MockData/Professions.json";
            string fileContent;

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
            }
            var root = JsonConvert.DeserializeObject<ProfessionModelRoot>(fileContent);

            return root?.Professions ?? new List<ProfessionModel>();
        }

        internal async Task<IEnumerable<string>> GetProfileTypes()
        {
            string filePath = $"MockData/ProfileTypes.json";
            return await ReadFromFileAsync(filePath);
        }

        internal async Task<IEnumerable<ScienceModel>> GetSciences()
        {
            string filePath = "MockData/Sciences.json";
            string fileContent;

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
            }
            var root = JsonConvert.DeserializeObject<SciencesRoot>(fileContent);

            return root?.Sciences ?? new List<ScienceModel>();
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
        }
    }
}
