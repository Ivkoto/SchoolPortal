using System.Data;
using Microsoft.Data.SqlClient;

namespace SchoolPortal.Api.Extensions
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync (CancellationToken cancellationToken = default);
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string connectionString;

        public DbConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);

            return connection;
        }
    }
}
