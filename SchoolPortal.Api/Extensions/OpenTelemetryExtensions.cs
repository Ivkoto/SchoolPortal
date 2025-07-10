using System.Diagnostics;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SchoolPortal.Api.Extensions;

public static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration, string? environmentName)
    {
        var openTelemetryEndpoint = configuration["OpenTelemetry:SigNozEndpoint"];

        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService("SchoolPortalApi")
            .AddAttributes(new KeyValuePair<string, object>[]
            {
                new("deployment.environment", environmentName ?? "Production"),
                new("service.instance.id", Environment.MachineName)
            });

        services
            .AddOpenTelemetry()
            .WithTracing(tracerProviderBuilder =>
            {
                tracerProviderBuilder
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        //TODO IvayloK: Including query parameters in logs can potentially expose sensitive information!
                        //If certain query parameters may contain sensitive data, implement logic to redact or exclude them!
                        options.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            var pathAndQuery = httpRequest.Path + httpRequest.QueryString;
                            activity.SetTag("http.target", pathAndQuery);
                        };
                    })
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddOtlpExporter(otpOptions =>
                    {
                        otpOptions.Endpoint = new Uri(openTelemetryEndpoint!);
                        otpOptions.Protocol = OtlpExportProtocol.Grpc;
                    });
            })
            .WithMetrics(metricProviderBuilder =>
            {
                metricProviderBuilder
                    .SetResourceBuilder(resourceBuilder)
                    .AddMeter("SchoolPortal.Health")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddOtlpExporter(otpOptions =>
                    {
                        otpOptions.Endpoint = new Uri(openTelemetryEndpoint!);
                        otpOptions.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

        services
            .AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();

                logging.AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(resourceBuilder);
                    options.IncludeFormattedMessage = true;
                    options.IncludeScopes = true;

                    options.AddOtlpExporter(otpOptions =>
                    {
                        otpOptions.Endpoint = new Uri(openTelemetryEndpoint!);
                        otpOptions.Protocol = OtlpExportProtocol.Grpc;
                    });
                });
            });
    }
}
