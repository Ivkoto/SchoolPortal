using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using SchoolPortal.Api.Models;
using static System.Net.Mime.MediaTypeNames;

namespace SchoolPortal.Api.Repositories
{
    public interface IProfileRepository
    {
        Task<List<ProfileModel>> GetFilteredProfiles(LookupProfilesRequest filters, CancellationToken cancellationToken);
        Task<List<ScienceModel>> GetAllSciences(CancellationToken cancellationToken);
        Task<List<ProfessionalDirectionModel>> GetProfessionalDirectionsByScienceId(int scienceId, CancellationToken cancellationToken);
        Task<List<ProfessionModel>> GetProfessionsByProfessionalDirectionId(int professionalDirectionId, CancellationToken cancellationToken);
    }
    public class ProfileRepository : IProfileRepository
    {
        private readonly IConfiguration configuration;
        private readonly Serilog.ILogger logger;

        public ProfileRepository(IConfiguration configuration, Serilog.ILogger logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        public async Task<List<ProfileModel>> GetFilteredProfiles(LookupProfilesRequest filters, CancellationToken cancellationToken)
        {
            var connectionString = configuration.GetConnectionString("DatabaseConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();

            parameters.Add("@Area", filters.Area ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@Settlement", filters.Settlement ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@Region", filters.Region ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@Neighbourhood", filters.Neighbourhood ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@SchoolYear", filters.SchoolYear ?? (object)DBNull.Value, DbType.Int32);
            parameters.Add("@Grade", filters.Grade ?? (object)DBNull.Value, DbType.Int32);
            parameters.Add("@ProfileType", filters.ProfileType ?? (object)DBNull.Value, DbType.String);
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

        public async Task<List<ScienceModel>> GetAllSciences(CancellationToken cancellationToken)
        {
            var connectionString = configuration.GetConnectionString("DatabaseConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            return (await connection.QueryAsync<ScienceModel>(
                        sql: "[Application].[usp_GetAllSciences]",
                        commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<ProfessionalDirectionModel>> GetProfessionalDirectionsByScienceId(
            int scienceId, CancellationToken cancellationToken)
        {
            var connectionString = configuration.GetConnectionString("DatabaseConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            return(await connection.QueryAsync<ProfessionalDirectionModel>(
                sql: "[Application].[usp_ProfessionalDirectionsByScienceId]",
                param: new { ScienceId = scienceId},
                commandType: CommandType.StoredProcedure
            )).ToList();
        }

        public async Task<List<ProfessionModel>> GetProfessionsByProfessionalDirectionId(
            int professionalDirectionId, CancellationToken cancellationToken)
        {
            var connectionString = configuration.GetConnectionString("DatabaseConnection");

            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            return(await connection.QueryAsync<ProfessionModel>(
                sql: "[Application].[usp_ProfessionsByProfessionalDirectionId]",
                param: new { ProfessionalDirectionId = professionalDirectionId},
                commandType: CommandType.StoredProcedure
            )).ToList();
        }
    }
}
