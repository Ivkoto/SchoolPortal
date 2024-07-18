using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = LoadConfiguration();
var logger = Log.Logger;

logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    //.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SchoolPortal API", Version = "v1" });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

// Middleware to log requests
app.Use(async (context, next) =>
{
    Log.Information("Incoming request: {Method} {Path} {QueryString} {Body}",
        context.Request.Method,
        context.Request.Path,
        context.Request.QueryString,
        context.Request.Body);

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger()
   .UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolPortal API v1"))
   .UseHttpsRedirection()
   .UseStaticFiles();


// Controllers
app.MapGet("/", async context =>
{
    var htmlFilePath = Path.Combine(builder.Environment.WebRootPath, "index.html");
    var htmlContent = await File.ReadAllTextAsync(htmlFilePath);
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(htmlContent);
});

app.MapGet("/ping", async (HealthCheckService healthCheckService) =>
{
    var healthReport = await healthCheckService.CheckHealthAsync();
    var result = new
    {
        Status = healthReport.Status.ToString(),
        Results = healthReport.Entries.Select(e => new { key = e.Key, value = e.Value.Status.ToString() })
    };
    return Results.Ok(result);
});

app.MapGet("/ping/details", async (HealthCheckService healthCheckService, CancellationToken cancellationToken) =>
{
    const string apiName = "School Portal API Database Healthy.";
    const string type = "DB Connection check";
    const string title = "An error occurred while processing a request to the database.";
    const string errorMessage = "Could not connect to database";
    const string successMessage = "Connection to the database established.";

    var connectionString = configuration.GetConnectionString("DatabaseConnection");

    try
    {
        await CheckDbConnection(connectionString, healthCheckService, cancellationToken);
        logger.Information(successMessage);
        return Results.Ok(apiName);
    }
    catch (Exception e)
    {
        logger.Error(errorMessage, e);
        return Results.Problem(detail: errorMessage, statusCode: 500, type: type, title:title);
    }
});

app.Run();

//Private methods
static IConfigurationRoot LoadConfiguration()
    => new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

static async Task CheckDbConnection(string connectionString, HealthCheckService healthCheckService, CancellationToken cancellationToken)
{
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync(cancellationToken);
    var command = new SqlCommand("Select 1", connection);
    await command.ExecuteNonQueryAsync(cancellationToken);
    await connection.CloseAsync();
}
