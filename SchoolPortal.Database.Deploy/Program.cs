using DbUp;
using DbUp.Engine;
using DbUp.Support;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Reflection;

namespace SchoolPortal.Database.Deploy
{
    public class Program
    {
        public static int Main()
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(LoadConfiguration())
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            var configuration = LoadConfiguration();

            try
            {
                var connectionString = configuration.GetConnectionString("DatabaseConnection");
                var environment = configuration["Environment"];
                var result = DeployDatabase(connectionString, environment);

                Log.Information("Deployment {Result}", result ? "Successful" : "Failed");

                return result ? 0 : 1;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occurred during deployment.");

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IConfigurationRoot LoadConfiguration()
            => new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .Build();

        private static bool DeployDatabase(string connectionString, string environment)
        {
            if (environment == "LocalRun")
            {
                ResetDatabase(connectionString);
            }

            var assembly = Assembly.GetExecutingAssembly();

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsAndCodeEmbeddedInAssembly(assembly, script => script.Contains(".Scripts."),
                    new SqlScriptOptions { RunGroupOrder = 1, ScriptType = ScriptType.RunOnce })
                .WithScriptsAndCodeEmbeddedInAssembly(assembly, script => script.Contains(".AlwaysRun.Functions."),
                    new SqlScriptOptions { RunGroupOrder = 2, ScriptType = ScriptType.RunAlways })
                .WithScriptsAndCodeEmbeddedInAssembly(assembly, script => script.Contains(".AlwaysRun.Views."),
                    new SqlScriptOptions { RunGroupOrder = 3, ScriptType = ScriptType.RunAlways })
                .WithScriptsAndCodeEmbeddedInAssembly(assembly, script => script.Contains(".AlwaysRun.StoredProcedures."),
                    new SqlScriptOptions { RunGroupOrder = 4, ScriptType = ScriptType.RunAlways })
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            Console.ForegroundColor = result.Successful ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(result.Successful ? "Deployment Successful" : "Deployment Failed");
            Console.ResetColor();

            return result.Successful;
        }

        private static void ResetDatabase(string connectionString)
        {
            DropDatabase.For.SqlDatabase(connectionString);
            EnsureDatabase.For.SqlDatabase(connectionString);
        }
    }
}
