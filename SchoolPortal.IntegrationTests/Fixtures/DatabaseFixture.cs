using DbUp;
using DbUp.Engine;
using DbUp.Support;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Respawn;

namespace SchoolPortal.IntegrationTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }
    private readonly Respawner respawner;
    private readonly SqlConnection connection;

    public DatabaseFixture()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();

        ConnectionString = configuration.GetConnectionString("DatabaseConnection")!;

        EnsureDatabase.For.SqlDatabase(ConnectionString);

        var assembly = typeof(Database.Deploy.Program).Assembly;

        var upgrader = DeployChanges.To
            .SqlDatabase(ConnectionString)
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

        if (!result.Successful)
        {
            throw new Exception("Database migration failed", result.Error);
        }

        // Respawner
        connection = new SqlConnection(ConnectionString);
        connection.Open();

        respawner = Respawner
            .CreateAsync(connection, new RespawnerOptions{ DbAdapter = DbAdapter.SqlServer})
            .GetAwaiter()
            .GetResult();
    }

    public async Task ResetDatabaseAsync()
    {
        await respawner.ResetAsync(connection);
    }

    public void Dispose()
    {
        DropDatabase.For.SqlDatabase(ConnectionString);
        connection.Dispose();
    }
}

[CollectionDefinition("IntegrationTestsCollection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    /*
    This class is used to apply [CollectionDefinition] and the ICollectionFixture<> interface.
    Defines a collection of tests that share the same context(the database fixture).
    This ensures that tests are not run in parallel, avoiding conflicts.
    */
}
