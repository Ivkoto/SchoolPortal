using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace SchoolPortal.IntegrationTests.Fixtures;

public class SchoolPortalApiApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.Sources.Clear();

            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            config.AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true);
            config.AddEnvironmentVariables();
        });

        builder.ConfigureServices(services =>
        {
            // Configure services if needed
            // Modify or replace services for testing - mock services or replace implementations
        });

        return base.CreateHost(builder);
    }
}

/*
Microsoft Docs Important:
For apps that use the minimal hosting model (available starting in ASP.NET Core 6.0),
WebApplicationFactory now runs the application's Program class (just like when the app is run for real).
In order to customize IHostBuilder in integration tests, override WebApplicationFactory.CreateHost(IHostBuilder)
and configure as needed.
*/