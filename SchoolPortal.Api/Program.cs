using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SchoolPortal.Api.Endpoints;
using SchoolPortal.Api.Extensions;
using SchoolPortal.Api.Infrastructure.HealthChecks;

var builder = WebApplication.CreateBuilder(args);
var environmentName = builder.Environment.EnvironmentName ?? "Production";

var configuration = builder.Configuration;

builder.Services.ServiceCollectionExtensions(configuration);
builder.Services.AddEndpoints(typeof(IEndpoint));
builder.Services.AddHealthCheck();
builder.Services.AddOpenTelemetry(configuration, environmentName);
builder.Services.TryAddSingleton<IHealthCheckPublisher, TelemetryHealthCheckPublisher>();

var app = builder.Build();

app.WebApplicationExtensions();
app.UseEndpoints();
app.UseCors("AllowedOriginsPolicy");

app.MapHealthChecks("/api/v1/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    AllowCachingResponses = false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.MapGet("/", async context =>
{
    var htmlFilePath = Path.Combine(builder.Environment.WebRootPath, "index.html");
    var htmlContent = await File.ReadAllTextAsync(htmlFilePath);
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(htmlContent);
});

app.Run();

public partial class Program { }