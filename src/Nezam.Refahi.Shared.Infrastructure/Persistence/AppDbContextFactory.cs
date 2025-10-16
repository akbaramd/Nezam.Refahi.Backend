using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nezam.Refahi.Shared.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for AppDbContext
/// Used by EF Core tools for migrations and database operations
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        
        
     // Get the connection string from configuration or use a default connection
        var connectionString =
            "data source=192.168.200.7\\SQL2019;initial catalog=Nezam.Refahi;persist security info=True;user id=sa;password=vhdSAM@15114;MultipleActiveResultSets=True;App=EntityFramework;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.EnableSensitiveDataLogging();

        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory__Shared", "shared");
        });

        return new AppDbContext(optionsBuilder.Options);
    }
}
