using Microsoft.OpenApi.Models;
using SchoolPortal.Api.Middlewares;

namespace SchoolPortal.Api.Extensions;

public static class StartupExtensions
{
    public static void ServiceCollectionExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        AddSwagger(services, configuration);
        AddCors(services, configuration);

        services.AddSingleton<IDbConnectionFactory>(
            _ => new DbConnectionFactory(configuration.GetConnectionString("DatabaseConnection")!));
    }

    public static void WebApplicationExtensions(this WebApplication app)
    {
        ConfigureSwagger(app);
        ConfigureMiddleware(app);
    }

    private static void AddSwagger(IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(s =>
        {
            s.SupportNonNullableReferenceTypes();

            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SchoolPortal API",
                Description = "School Portal API for education information management",
                Contact = new OpenApiContact
                {
                    Name = "© DevOcean Solutions",
                    Email = "",
                    Url = new Uri("https://devocean.services/")
                },
                Version = "v1"
            });
            s.EnableAnnotations();
        });
    }

    ///<summary>
    ///If there are no allowed origins configured, default it creates a restrictive CORS policy that blocks all cross-origin requests.
    /// </summary>
    private static void AddCors(IServiceCollection services, IConfiguration configuration)
    {        
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("AllowedOriginsPolicy", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .WithMethods("GET", "POST")
                      .AllowAnyHeader()
                      .SetPreflightMaxAge(TimeSpan.FromHours(2));
            });

            options.AddPolicy("PaginationPolicy", policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .WithMethods("GET", "POST")
                      .AllowAnyHeader()
                      .WithExposedHeaders("X-Pagination")
                      .SetPreflightMaxAge(TimeSpan.FromHours(2));
            });
        });
    }

    private static void ConfigureSwagger(WebApplication app)
    {
        var enableSwaggerInProduction = app.Configuration.GetValue<bool>("Swagger:EnableInProduction", false);
        var productionRoute = app.Configuration.GetValue<string>("Swagger:ProductionRoute", "api-docs");
        
        if (app.Environment.IsDevelopment() || (app.Environment.IsProduction() && enableSwaggerInProduction))
        {
            // Use different route in production for security
            var swaggerRoute = app.Environment.IsProduction() ? productionRoute : "swagger";
            
            app.UseSwagger(c =>
            {
                c.RouteTemplate = $"{swaggerRoute}/{{documentName}}/swagger.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                {
                    var serverUrl = $"{httpReq.Scheme}://{httpReq.Host.Value}";
                    swaggerDoc.Servers = new List<OpenApiServer>
                    {
                        new OpenApiServer { Url = serverUrl }
                    };
                });
            });
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint($"/{swaggerRoute}/v1/swagger.json", "SchoolPortal API v1");
                s.RoutePrefix = swaggerRoute;
                s.DocumentTitle = app.Environment.IsProduction() ? 
                    "SchoolPortal API Documentation" : 
                    "SchoolPortal API Documentation (Development)";
                s.DisplayRequestDuration();
                s.EnableTryItOutByDefault();
                s.ConfigObject.AdditionalItems.Add("syntaxHighlight", true);
                
                // Production-specific customizations
                if (app.Environment.IsProduction())
                {
                    s.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                    s.DefaultModelsExpandDepth(-1); // Hide models section by default in production
                }
            });
        }
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
