using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SchoolPortal.Api.Infrastructure.HealthChecks;

public class ApiHealthCheck : IHealthCheck
{
    private readonly ILogger<ApiHealthCheck> logger;

    public ApiHealthCheck(ILogger<ApiHealthCheck> logger)
    {
        this.logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("API health check executed successfully");
            return Task.FromResult(HealthCheckResult.Healthy("API is running"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "API health check failed");
            return Task.FromResult(HealthCheckResult.Unhealthy("API health check failed", ex));
        }
    }
}
