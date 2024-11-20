using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SchoolPortal.Api.Endpoints;
using SchoolPortal.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.ServiceCollectionExtensions(configuration);
builder.Services.AddEndpoints(typeof(IEndpoint));
builder.Services.AddHealthChecks()
    .AddSqlServer(configuration.GetConnectionString("DatabaseConnection")!);

var app = builder.Build();

app.WebApplicationExtensions();
app.UseEndpoints();
app.UseCors("AllowedOriginsPolicy");

app.MapHealthChecks("/api/v1/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/", async context =>
{
    var htmlFilePath = Path.Combine(builder.Environment.WebRootPath, "index.html");
    var htmlContent = await File.ReadAllTextAsync(htmlFilePath);
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(htmlContent);
});

app.Run();
