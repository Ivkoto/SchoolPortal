using System.Data;
using Dapper;
using SchoolPortal.Api.Extensions;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Repositories
{
    public interface IInstitutionRepository
    {
        Task<InstitutionModel> GetInstitutionAsync(int institutionId);
        Task<List<ProfileModel>> GetInstitutionProfiles(int institutionId, int schoolYear, int? grade);
    }

    public class InstitutionRepository : IInstitutionRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public InstitutionRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<InstitutionModel> GetInstitutionAsync(int institutionId)
        {
            var connection = await connectionFactory.CreateConnectionAsync();

            var institution = await connection.QuerySingleOrDefaultAsync<InstitutionModel>(
                              sql: "[Application].[usp_GetInstitutionById]",
                              param: new { institutionId },
                              commandType: CommandType.StoredProcedure);

            return institution ?? throw new KeyNotFoundException($"No Institution found with the ID {institutionId}");
        }

        public async Task<List<ProfileModel>> GetInstitutionProfiles(int institutionId, int schoolYear, int? grade)
        {
            var connection = await connectionFactory.CreateConnectionAsync();
            var parameters = new DynamicParameters();

            parameters.Add("@InstitutionId", institutionId, DbType.Int32);
            parameters.Add("@SchoolYear", schoolYear, DbType.Int32);
            parameters.Add("@Grade", grade ?? (object)DBNull.Value, DbType.Int32);

            return (await connection.QueryAsync<ProfileModel>(
                    sql: "[Application].[usp_GetFilteredProfiles]",
                    param: parameters,
                    commandType: CommandType.StoredProcedure
            )).ToList();
        }
    }
}
