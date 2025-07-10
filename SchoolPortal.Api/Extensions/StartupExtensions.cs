using Microsoft.OpenApi.Models;
using SchoolPortal.Api.Middlewares;

namespace SchoolPortal.Api.Extensions;

public static class StartupExtensions
{
    public static void ServiceCollectionExtensions(this IServiceCollection services, IConfiguration configuration)
    {
        AddSwager(services);
        AddCors(services, configuration);

        services.AddSingleton<IDbConnectionFactory>(
            _ => new DbConnectionFactory(configuration.GetConnectionString("DatabaseConnection")!));
    }

    public static void WebApplicationExtensions(this WebApplication app)
    {
        ConfigureSwager(app);
        ConfigureMiddleware(app);
    }

    private static void AddSwager(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SchoolPortal API",
                Version = "v1",
                Description = "School Portal API for education information management",
                Contact = new OpenApiContact
                {
                    Name = "SchoolPortal Team"
                }
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

    private static void ConfigureSwager(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "swagger/{documentName}/swagger.json";
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
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolPortal API v1");
                s.RoutePrefix = "swagger";
                s.DocumentTitle = "SchoolPortal API Documentation";
                s.DisplayRequestDuration();
                s.EnableTryItOutByDefault();
                s.ConfigObject.AdditionalItems.Add("syntaxHighlight", true);
            });
        }
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
