using System.Data;
using Dapper;
using SchoolPortal.Api.Extensions;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Repositories
{
    public interface IProfileRepository
    {
        Task<List<ProfileModel>> GetFilteredProfiles(GetFilteredProfilesRequest filters);
        Task<ProfileModel> GetProfileById(int profileId);
        Task<List<ScienceModel>> GetAllSciences();
        Task<List<ProfessionalDirectionModel>> GetProfessionalDirectionsByScienceId(int scienceId);
        Task<List<ProfessionModel>> GetProfessionsByProfessionalDirectionId(int professionalDirectionId);
        Task<List<SpecialtyModel>> GetSpecialtiesByProfessionId(int professionId);
        Task<List<ExamStageScoresModel>> GetAllExamStageScores(int ProfileId, int SchoolYear);
    }

    public class ProfileRepository : IProfileRepository
    {
        private readonly Serilog.ILogger logger;
        private readonly IDbConnectionFactory connectionFactory;

        public ProfileRepository(Serilog.ILogger logger, IDbConnectionFactory connectionFactory)
        {
            this.logger = logger;
            this.connectionFactory = connectionFactory;
        }

        public async Task<List<ProfileModel>> GetFilteredProfiles(GetFilteredProfilesRequest filters)
        {
            var connection = await connectionFactory.CreateConnectionAsync();
            var parameters = new DynamicParameters();

            parameters.Add("@SchoolYear", filters.SchoolYear, DbType.Int32);
            parameters.Add("@Grade", filters.Grade, DbType.Int32);
            parameters.Add("@Settlement", filters.Settlement ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@Neighbourhood", filters.Neighbourhood ?? (object)DBNull.Value, DbType.String);

            if (filters.GeoLocationFilter != null)
            {
                parameters.Add("@Latitude", filters.GeoLocationFilter.Latitude, DbType.Decimal);
                parameters.Add("@Longitude", filters.GeoLocationFilter.Longitude, DbType.Decimal);
                parameters.Add("@Radius", filters.GeoLocationFilter.Radius, DbType.Decimal);
            }

            if (filters.ProfileType is not null)
            {
                var isProfessional = filters.ProfileType.ToLower() == CustomEnums.ProfileTypes.Professional ? 1 : 0;
                parameters.Add("@IsProfessional", isProfessional, DbType.Int32);
            }
            else
            {
                parameters.Add("@IsProfessional", (object)DBNull.Value, DbType.Int32);
            }

            parameters.Add("@SpecialtyId", filters.SpecialtyId ?? (object)DBNull.Value, DbType.Int32);
            parameters.Add("@ProfessionId", filters.ProfessionId ?? (object)DBNull.Value, DbType.Int32);
            parameters.Add("@ProfessionalDirectionId", filters.ProfessionalDirectionId ?? (object)DBNull.Value, DbType.Int32);
            parameters.Add("@ScienceId", filters.ScienceId ?? (object)DBNull.Value, DbType.Int32);

            return (await connection.QueryAsync<ProfileModel>(
                    sql: "[Application].[usp_GetFilteredProfiles]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<ProfileModel> GetProfileById(int profileId)
        {
            var connection = await connectionFactory.CreateConnectionAsync();

            var profile = await connection.QuerySingleOrDefaultAsync<ProfileModel>(
                          sql: "[Application].[usp_GetProfileById]",
                          param: new { profileId },
                          commandType: CommandType.StoredProcedure);

            return profile ?? throw new KeyNotFoundException($"No Profile found with the ID {profileId}");
        }

        public async Task<List<ScienceModel>> GetAllSciences()
        {
            var connection = await connectionFactory.CreateConnectionAsync();

            return (await connection.QueryAsync<ScienceModel>(
                    sql: "[Application].[usp_GetAllSciences]",
                    commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<ProfessionalDirectionModel>> GetProfessionalDirectionsByScienceId(int scienceId)
        {
            var connection = await connectionFactory.CreateConnectionAsync();


            return(await connection.QueryAsync<ProfessionalDirectionModel>(
                   sql: "[Application].[usp_GetProfessionalDirectionsByScienceId]",
                   param: new { ScienceId = scienceId},
                   commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<ProfessionModel>> GetProfessionsByProfessionalDirectionId(int professionalDirectionId)
        {
            var connection = await connectionFactory.CreateConnectionAsync();

            return(await connection.QueryAsync<ProfessionModel>(
                   sql: "[Application].[usp_GetProfessionsByProfessionalDirectionId]",
                   param: new { ProfessionalDirectionId = professionalDirectionId},
                   commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<SpecialtyModel>> GetSpecialtiesByProfessionId(int professionId)
        {
            var connection = await connectionFactory.CreateConnectionAsync();

            var parameters = new DynamicParameters();

            parameters.Add("@IsProfessional", professionId > 0 ? 1 : 0, DbType.Int32);
            parameters.Add("@ProfessionId", professionId > 0 ? professionId : (object)DBNull.Value, DbType.Int32);

            return (await connection.QueryAsync<SpecialtyModel>(
                    sql: "[Application].[usp_GetSpecialtiesByProfessionId]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<ExamStageScoresModel>> GetAllExamStageScores(int profileId, int schoolYear)
        {
            var connection = await connectionFactory.CreateConnectionAsync();

            return(await connection.QueryAsync<ExamStageScoresModel> (
                   sql: "[Application].[usp_GetExamStageScoresByProfileId]",
                   param: new { ProfileId = profileId, SchoolYear = schoolYear },
                   commandType: CommandType.StoredProcedure
            )).ToList();
        }
    }
}
