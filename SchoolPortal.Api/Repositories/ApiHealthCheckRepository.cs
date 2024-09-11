using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SchoolPortal.Api.Services
{
    public interface IApiHealthCheckRepository
    {
        Task<IResult> Ping(CancellationToken cancellationToken);
        Task<IResult> PingDetails(CancellationToken cancellationToken);
    }

    public class ApiHealthCheckRepository : IApiHealthCheckRepository
    {
        private readonly IConfiguration configuration;
        private readonly Serilog.ILogger logger;
        private readonly HealthCheckService healthCheckService;

        public ApiHealthCheckRepository(IConfiguration configuration, Serilog.ILogger logger, HealthCheckService healthCheckService)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.healthCheckService = healthCheckService;
        }

        public async Task<IResult> Ping(CancellationToken cancellationToken)
        {
            const string successMessage = "Connection to the API established.";

            var healthReport = await healthCheckService.CheckHealthAsync();

            logger.Information(successMessage);
            return Results.Ok(successMessage);
        }

        public async Task<IResult> PingDetails(CancellationToken cancellationToken)
        {
            const string errorMessage = "Could not connect to database";
            const string successMessage = "Connection to the database established.";

            var connectionString = configuration.GetConnectionString("DatabaseConnection");

            var healthReport = await healthCheckService.CheckHealthAsync(c => c.Tags.Contains("Database"));
            var databaseStatus = healthReport.Entries.FirstOrDefault(e => e.Key == "Database").Value.Status;

            try
            {
                await CheckDbConnection(connectionString, cancellationToken);
                logger.Information(successMessage);
                return TypedResults.Ok(successMessage);
            }
            catch (Exception e)
            {
                logger.Error(errorMessage, e);
                return TypedResults.Problem(detail: errorMessage, statusCode: 500);
            }
        }

        private async Task CheckDbConnection(
            string connectionString,
            CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync(cancellationToken);
            var command = new SqlCommand("Select 1", connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
            await connection.CloseAsync();
        }
    }
}
