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
        ConfigureMiddlewares(app);
    }

    private static void AddSwager(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo { Title = "SchoolPortal API", Version = "v1" });
            s.EnableAnnotations();
        });
    }

    private static void AddCors(IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

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
        app.UseSwagger();
        app.UseSwaggerUI(s =>
        {
            s.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolPortal API v1");
        });
    }

    private static void ConfigureMiddlewares(WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
