using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace SchoolPortal.Api.Extensions;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string connectionString;

    public DbConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public async Task<DbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
