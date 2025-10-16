using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nezam.Refahi.Surveying.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for SurveyDbContext to enable EF Core tools (migrations, etc.)
/// </summary>
public class SurveyDbContextFactory : IDesignTimeDbContextFactory<SurveyDbContext>
{
    public SurveyDbContext CreateDbContext(string[] args)
    {
        // Get the connection string from configuration or use a default connection
        var connectionString =
            "data source=192.168.200.7\\SQL2019;initial catalog=Nezam.Refahi;persist security info=True;user id=sa;password=vhdSAM@15114;MultipleActiveResultSets=True;App=EntityFramework;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<SurveyDbContext>();
        optionsBuilder.EnableSensitiveDataLogging();

        // Use SQL Server as the database provider
        optionsBuilder.UseSqlServer(
            connectionString,
            x => x.MigrationsHistoryTable("__EFMigrationsHistory__Surveying", "surveying"));

        return new SurveyDbContext(optionsBuilder.Options);
    }
}
