using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nezam.Refahi.Facilities.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for FacilitiesDbContext to enable EF Core tools (migrations, etc.)
/// </summary>
public class FacilitiesDbContextFactory : IDesignTimeDbContextFactory<FacilitiesDbContext>
{
    public FacilitiesDbContext CreateDbContext(string[] args)
    {
        // Get the connection string from configuration or use a default connection
        var connectionString =
            "data source=192.168.200.7\\SQL2019;initial catalog=Nezam.Refahi;persist security info=True;user id=sa;password=vhdSAM@15114;MultipleActiveResultSets=True;App=EntityFramework;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<FacilitiesDbContext>();
        optionsBuilder.EnableSensitiveDataLogging();

        // Use SQL Server as the database provider
        optionsBuilder.UseSqlServer(
            connectionString,
            x => x.MigrationsHistoryTable("__EFMigrationsHistory__Facilities", "facilities"));

        return new FacilitiesDbContext(optionsBuilder.Options);
    }
}
