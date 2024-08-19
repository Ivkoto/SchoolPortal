using System.Text;
using Newtonsoft.Json;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Endpoints
{
    public class ProfileEndpoints : IEndpoint
    {
        public void MapEndpoints(WebApplication app)
        {
            app.MapGet("/Profiles/GetAll", GetAllProfile);
            app.MapGet("/Profiles/GetAllSciences", GetAllSciences);
            app.MapGet("/Profiles/GetAllProfessions", GetAllProfessions);
            app.MapGet("/Profiles/GetAllSpecialties", GetAllSpecialties);
            app.MapGet("/Profiles/GetAllProfessionalDirections", GetAllProfessionalDirections);
            app.MapGet("/Profiles/GetAllFirstForeignLanguages", GetAllFirstForeignLanguages);
            app.MapGet("/Profiles/GetAllStudyMethods", GetAllStudyMethods);
            app.MapGet("/Profiles/GetAllStudyPeriods", GetAllStudyPeriods);
        }

        internal async Task<IEnumerable<ProfileModel>> GetAllProfile()
        {
            string filePath = "MockData/Profiles.json";
            string fileContent;

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            {
                fileContent = await reader.ReadToEndAsync();
            }
            var root = JsonConvert.DeserializeObject<ProfilesRoot>(fileContent);

            return root?.Profiles ?? new List<ProfileModel>();
        }

        internal async Task<IEnumerable<string>> GetAllStudyPeriods()
        {
            string filePath = $"MockData/StudyPeriods.json";
            return await ReadFromFileAsync(filePath);
        }

        internal async Task<IEnumerable<string>> GetAllStudyMethods()
        {
            string filePath = $"MockData/StudyMethods.json";
            return await ReadFromFileAsync(filePath);
        }

        internal async Task<IEnumerable<string>> GetAllFirstForeignLanguages()
        {
            string filePath = $"MockData/FirstForeignLanguages.json";
            return await ReadFromFileAsync(filePath);
        }

        internal async Task<IEnumerable<string>> GetAllProfessionalDirections()
        {
            string filePath = $"MockData/ProfessionalDirections.json";
            return await ReadFromFileAsync(filePath);
        }

        internal async Task<IEnumerable<string>> GetAllSciences()
        {
            string filePath = $"MockData/Sciences.json";
            return await ReadFromFileAsync(filePath);
        }

        internal async Task<IEnumerable<string>> GetAllProfessions()
        {
            string filePath = $"MockData/Professions.json";
            return await ReadFromFileAsync(filePath); ;
        }

        internal async Task<IEnumerable<string>> GetAllSpecialties()
        {
            string filePath = $"MockData/Specialties.json";
            return await ReadFromFileAsync(filePath);
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
        }
    }
}
