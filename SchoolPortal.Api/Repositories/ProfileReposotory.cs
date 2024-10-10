using System.Data;
using Dapper;
using SchoolPortal.Api.Extensions;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Repositories
{
    public interface IProfileRepository
    {
        Task<List<ProfileModel>> GetFilteredProfiles(LookupProfilesRequest filters);
        Task<List<ScienceModel>> GetAllSciences();
        Task<List<ProfessionalDirectionModel>> GetProfessionalDirectionsByScienceId(int scienceId);
        Task<List<ProfessionModel>> GetProfessionsByProfessionalDirectionId(int professionalDirectionId);
        Task<List<SpecialtyModel>> GetSpecialtiesByProfessionId(int professionId);
    }
    public class ProfileRepository : IProfileRepository
    {
        private readonly Serilog.ILogger logger;
        private readonly IDbConnectionFactory connectionFactory;

        public ProfileRepository(IConfiguration configuration, Serilog.ILogger logger, IDbConnectionFactory connectionFactory)
        {
            this.logger = logger;
            this.connectionFactory = connectionFactory;
        }

        public async Task<List<ProfileModel>> GetFilteredProfiles(LookupProfilesRequest filters)
        {
            var connection = await connectionFactory.CreateConnectionAsync();

            var parameters = new DynamicParameters();

            parameters.Add("@Area", filters.Area ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@Settlement", filters.Settlement ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@Region", filters.Region ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@Neighbourhood", filters.Neighbourhood ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@SchoolYear", filters.SchoolYear ?? (object)DBNull.Value, DbType.Int32);
            parameters.Add("@Grade", filters.Grade ?? (object)DBNull.Value, DbType.Int32);

            if (filters.ProfileType is not null)
            {
                var isProfessional = filters.ProfileType.ToLower() == CustomEnums.ProfileType.Professional ? 1 : 0;
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

            if (filters.GeoLocationFilter != null)
            {
                parameters.Add("@Latitude", filters.GeoLocationFilter.Latitude, DbType.Decimal);
                parameters.Add("@Longitude", filters.GeoLocationFilter.Longitude, DbType.Decimal);
                parameters.Add("@Radius", filters.GeoLocationFilter.Radius, DbType.Decimal);
            }

            return (await connection.QueryAsync<ProfileModel>(
                       sql: "[Application].[usp_GetFilteredProfiles]",
                       param: parameters,
                       commandType: CommandType.StoredProcedure
            )).ToList();
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
                sql: "[Application].[usp_ProfessionalDirectionsByScienceId]",
                param: new { ScienceId = scienceId},
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<ProfessionModel>> GetProfessionsByProfessionalDirectionId(int professionalDirectionId)
        {
            var connection = await connectionFactory.CreateConnectionAsync();

            return(await connection.QueryAsync<ProfessionModel>(
                sql: "[Application].[usp_ProfessionsByProfessionalDirectionId]",
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
                sql: "[Application].[usp_SpecialtiesByProfessionId]",
                param: parameters,
                commandType: CommandType.StoredProcedure
            )).ToList();
        }
    }
}
