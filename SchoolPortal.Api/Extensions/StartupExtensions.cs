using Microsoft.OpenApi.Models;
using SchoolPortal.Api.Middlewares;
using Serilog;

namespace SchoolPortal.Api.Extensions
{
    public static class StartupExtensions
    {
        private static IConfiguration? configuration;

        public static void ServiceCollectionExtensions(this IServiceCollection services)
        {
            // Add Configuration
            configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            services.AddSingleton(configuration);

            // Add Swager
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "SchoolPortal API", Version = "v1" });
                s.EnableAnnotations();
            });

            // Add Logger
            var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    //.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

            services.AddSingleton<Serilog.ILogger>(logger);

            // Add DB Connection Factory
            services.AddSingleton<IDbConnectionFactory>(
                _ => new DbConnectionFactory(configuration.GetConnectionString("DatabaseConnection")!));


            // Add CORS configuration
            var allowedOrigins = new[] {
                "https://eduinfo.azurewebsites.net",
                "https://eduinfo-dev.azurewebsites.net",
                "http://localhost:3000"
            };

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

        public static void WebApplicationExtensions(this WebApplication app)
        {
            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(s => s.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolPortal API v1"))
               .UseHttpsRedirection()
               .UseStaticFiles();

            // Middlewares
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
