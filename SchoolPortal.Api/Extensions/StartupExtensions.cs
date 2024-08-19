using Serilog;

namespace SchoolPortal.Api.Extensions
{
    public static class StartupExtensions
    {
        private static IConfiguration configuration;

        public static void AddConfiguration(this IServiceCollection services)
        {
            configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

            services.AddSingleton(configuration);
        }

        public static void AddLogger(this IServiceCollection services)
        {
            var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    //.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

            services.AddSingleton<Serilog.ILogger>(logger);
        }
    }
}
