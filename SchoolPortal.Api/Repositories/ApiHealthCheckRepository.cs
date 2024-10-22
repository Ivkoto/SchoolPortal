using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SchoolPortal.Api.Extensions;

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
        private readonly IDbConnectionFactory connectionFactory;

        public ApiHealthCheckRepository(IConfiguration configuration, Serilog.ILogger logger,
            HealthCheckService healthCheckService, IDbConnectionFactory connectionFactory)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.healthCheckService = healthCheckService;
            this.connectionFactory = connectionFactory;
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

            //TODO @IvayloK - Not in use at the moment. There are no tagged services with for now.
            //var healthReport = await healthCheckService.CheckHealthAsync(c => c.Tags.Contains("Database"));
            //var databaseStatus = healthReport.Entries.FirstOrDefault(e => e.Key == "Database").Value.Status;

            try
            {
                var connection = await connectionFactory.CreateConnectionAsync();
                await connection.ExecuteAsync("Select 1", commandTimeout: null, transaction: null);
                logger.Information(successMessage);
                return TypedResults.Ok(successMessage);
            }
            catch (Exception e)
            {
                logger.Error(errorMessage, e);
                return TypedResults.Problem(detail: errorMessage, statusCode: 500);
            }
        }
    }
}
