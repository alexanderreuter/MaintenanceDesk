using MaintenanceDesk.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MaintenanceDesk.Tests.Integration;

public class MaintenanceDeskApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestDatabaseName = "MaintenanceDesk_IntegrationTests";

    private readonly string connectionString;

    public MaintenanceDeskApiFactory()
    {
        // The app's own sources: user secrets locally, environment variables in CI.
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

        var appConnectionString = configuration.GetConnectionString("MaintenanceDesk")
            ?? throw new InvalidOperationException("Connection string 'MaintenanceDesk' is not configured.");

        connectionString = new SqlConnectionStringBuilder(appConnectionString)
        {
            InitialCatalog = TestDatabaseName,
        }.ConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:MaintenanceDesk", connectionString);
    }

    public async Task InitializeAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<MaintenanceDeskDbContext>();

        // If the override above ever stops reaching the app, stop here rather than delete the development database.
        var database = db.Database.GetDbConnection().Database;
        if (database != TestDatabaseName)
        {
            throw new InvalidOperationException(
                $"Refusing to reset database '{database}'; expected the test database '{TestDatabaseName}'.");
        }

        await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}
