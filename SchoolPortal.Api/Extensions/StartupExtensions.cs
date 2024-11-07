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

            // Add Db Connection Factory
            services.AddSingleton<IDbConnectionFactory>(
                _ => new DbConnectionFactory(configuration.GetConnectionString("DatabaseConnection")!));
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
