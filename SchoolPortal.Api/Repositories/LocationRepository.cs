using Dapper;
using SchoolPortal.Api.Extensions;
using SchoolPortal.Api.Models;

namespace SchoolPortal.Api.Repositories
{
    public interface ILocationRepository
    {
        Task<List<NeighbourhoodModel>> GetNeighbourhoodsBySettlement(string Settlement);
    }

    public class LocationRepository : ILocationRepository
    {
        private readonly IDbConnectionFactory connectionFactory;

        public LocationRepository(IDbConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public async Task<List<NeighbourhoodModel>> GetNeighbourhoodsBySettlement(string settlement)
        {
            var connection = await connectionFactory.CreateConnectionAsync();

            return (await connection.QueryAsync<NeighbourhoodModel>(
                   sql: "[Application].[usp_GetNeighbourhoodsBySettlement]",
                   param: new { settlement },
                   commandType: System.Data.CommandType.StoredProcedure
            )).ToList();
        }
    }
}
