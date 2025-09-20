using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nezam.Refahi.Recreation.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for RecreationDbContext to enable EF Core tools (migrations, etc.)
/// </summary>
public class RecreationDbContextFactory : IDesignTimeDbContextFactory<RecreationDbContext>
{
    public RecreationDbContext CreateDbContext(string[] args)
    {
        // Build configuration
     
        // Get the connection string from configuration or use a default SQLite connection
        var connectionString = 
          "data source =192.168.200.7\\SQL2019;initial catalog=Nezam.Refahi;persist security info=True;user id=sa;password=vhdSAM@15114;MultipleActiveResultSets=True;App=EntityFramework;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<RecreationDbContext>();
        optionsBuilder.EnableSensitiveDataLogging();
        // Use SQL Server as the database provider
        optionsBuilder.UseSqlServer(
          connectionString,
          x => x.MigrationsHistoryTable("__EFMigrationsHistory__Recreation","recreation"));


        return new RecreationDbContext(optionsBuilder.Options);
    }
}