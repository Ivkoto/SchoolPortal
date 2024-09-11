using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Datadog.Logs;

namespace SchoolPortal.Database.Deploy.OLD.Logging
{
    internal class LoggingProvider
    {
        public static void InitializeLogger()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(GetLoggingLevelSwitch(configuration))
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.DatadogLogs(
                            apiKey: configuration.GetValue<string>("Serilog:WriteTo:1:Args:apiKey", configuration.GetValue<string>("DD_API_KEY", "missing api key")),
                            service: configuration.GetValue<string>("DD_SERVICE"),
                            host: Environment.MachineName,
                            tags: configuration.GetValue<string[]>("DD_TAGS"),
                            configuration: new DatadogConfiguration())
                .CreateLogger();
        }

        private static LoggingLevelSwitch GetLoggingLevelSwitch(IConfiguration configuration)
        {
            var logEventLevel = Enum.TryParse(configuration.GetValue<string>("DD_LOG_LEVEL"), true, out LogEventLevel loggingLevel)
                ? loggingLevel
                : LogEventLevel.Error;

            return new LoggingLevelSwitch(logEventLevel);
        }
    }
}
