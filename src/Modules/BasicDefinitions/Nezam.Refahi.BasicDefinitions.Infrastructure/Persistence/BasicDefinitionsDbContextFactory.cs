using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nezam.Refahi.BasicDefinitions.Infrastructure.Persistence;

/// <summary>
/// Factory for creating BasicDefinitionsDbContext instances for design-time operations
/// </summary>
public class BasicDefinitionsDbContextFactory : IDesignTimeDbContextFactory<BasicDefinitionsDbContext>
{
    public BasicDefinitionsDbContext CreateDbContext(string[] args)
    {
        // Get configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        var optionsBuilder = new DbContextOptionsBuilder<BasicDefinitionsDbContext>();
        optionsBuilder.UseSqlServer(
            connectionString,
            x => x.MigrationsHistoryTable("__EFMigrationsHistory__BasicDefinitions", "definitions"));

        return new BasicDefinitionsDbContext(optionsBuilder.Options);
    }
}