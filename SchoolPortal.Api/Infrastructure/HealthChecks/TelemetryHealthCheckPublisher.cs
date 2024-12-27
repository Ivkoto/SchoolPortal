using System.Diagnostics.Metrics;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SchoolPortal.Api.Infrastructure.HealthChecks;

public class TelemetryHealthCheckPublisher : IHealthCheckPublisher
{
    private readonly ILogger<TelemetryHealthCheckPublisher> logger;
    private readonly Meter meter;
    private readonly Counter<int> healthCheckCounter;
    private readonly Histogram<double> healthCheckDuration;

    public TelemetryHealthCheckPublisher(ILogger<TelemetryHealthCheckPublisher> logger)
    {
        this.logger = logger;
        meter = new Meter("SchoolPortal.Health");
        healthCheckCounter = meter.CreateCounter<int>("health_checks");
        healthCheckDuration = meter.CreateHistogram<double>("health_check_duration_ms");
    }

    public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
    {
        healthCheckCounter.Add(1, new KeyValuePair<string, object?>("status", report.Status.ToString()));

        foreach (var entry in report.Entries)
        {
            healthCheckDuration.Record(
                entry.Value.Duration.TotalMilliseconds,
                new KeyValuePair<string, object?>("check", entry.Key),
                new KeyValuePair<string, object?>("status", entry.Value.Status.ToString())
            );

            var logMessage = $"Health Check {entry.Key}: {entry.Value.Status} ({entry.Value.Duration.TotalMilliseconds}ms)";

            switch (entry.Value.Status)
            {
                case HealthStatus.Healthy:
                    logger.LogInformation(logMessage);
                    break;
                case HealthStatus.Degraded:
                    logger.LogWarning(logMessage);
                    break;
                case HealthStatus.Unhealthy:
                    logger.LogError(logMessage);
                    break;
                default:
                    logger.LogInformation(logMessage);
                    break;
            }
        }

        return Task.CompletedTask;
    }
}