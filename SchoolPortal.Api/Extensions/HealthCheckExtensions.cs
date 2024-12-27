using Microsoft.Extensions.Diagnostics.HealthChecks;
using SchoolPortal.Api.Infrastructure.HealthChecks;

namespace SchoolPortal.Api.Extensions;

public static class HealthCheckExtensions
{
    public static void AddHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<ApiHealthCheck>("api_health",
                failureStatus: HealthStatus.Degraded,
                tags: ["api"])
            .AddCheck<DbHealthCheck>("database_health",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["database"]);

        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(5);
            options.Period = TimeSpan.FromSeconds(120);
        });

        services.AddSingleton<IHealthCheckPublisher, TelemetryHealthCheckPublisher>();
    }
}
