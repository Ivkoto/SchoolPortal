using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SchoolPortal.Api.Extensions;

namespace SchoolPortal.Api.Infrastructure.HealthChecks;

public class DbHealthCheck : IHealthCheck
{
    private readonly IDbConnectionFactory dbConnectionFactory;
    private readonly ILogger<DbHealthCheck> logger;

    public DbHealthCheck(IDbConnectionFactory dbConnectionFactory, ILogger<DbHealthCheck> logger)
    {
        this.dbConnectionFactory = dbConnectionFactory;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            var result = await connection.ExecuteScalarAsync<int>("SELECT 1;");

            if (result == 1)
            {
                logger.LogInformation("Database connection is healthy");
                return HealthCheckResult.Healthy("Database connection is healthy");
            }
            else
            {
                logger.LogError("Unexpected result from database health check: {Result}", result);
                return HealthCheckResult.Unhealthy($"Unexpected result: {result}");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not connect to the database");
            return HealthCheckResult.Unhealthy("Database health check failed", ex);
        }
    }
}
