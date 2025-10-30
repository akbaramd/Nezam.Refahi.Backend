using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nezam.Refahi.Orchestrator.Persistence;

public class OrchestratorDbContextFactory : IDesignTimeDbContextFactory<OrchestratorDbContext>
{
    public OrchestratorDbContext CreateDbContext(string[] args)
    {
        var configuration = BuildConfiguration();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection connection string is not configured");

        var optionsBuilder = new DbContextOptionsBuilder<OrchestratorDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsHistoryTable("__EFMigrationsHistory__Orchestrator", "orchestrator");
        });

        return new OrchestratorDbContext(optionsBuilder.Options);
    }

    private static IConfiguration BuildConfiguration()
    {
        // Try to load settings from WebApi project to keep a single source of truth for connection strings
        var probe = Directory.GetCurrentDirectory();
        for (var i = 0; i < 6 && !string.IsNullOrEmpty(probe); i++)
        {
            var candidate = Path.Combine(probe, "src", "Nezam.Refahi.WebApi");
            var appsettings = Path.Combine(candidate, "appsettings.json");
            if (File.Exists(appsettings))
            {
                return new ConfigurationBuilder()
                    .SetBasePath(candidate)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();
            }

            var parent = Directory.GetParent(probe);
            probe = parent?.FullName ?? string.Empty;
        }

        // Fallback to environment only
        return new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();
    }
}


