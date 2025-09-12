using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nezam.Refahi.Membership.Infrastructure.Persistence;

/// <summary>
/// Factory for creating MembershipDbContext instances during design-time operations
/// such as migrations. This follows the DDD principle of keeping infrastructure concerns
/// separate from domain logic.
/// </summary>
public class MembershipDbContextFactory : IDesignTimeDbContextFactory<MembershipDbContext>
{
    public MembershipDbContext CreateDbContext(string[] args)
    {
        // Get the connection string from configuration or use a default connection
        var connectionString = 
          "data source =192.168.200.7\\SQL2019;initial catalog=Nezam.Refahi;persist security info=True;user id=sa;password=vhdSAM@15114;MultipleActiveResultSets=True;App=EntityFramework;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<MembershipDbContext>();
        
        // Use SQL Server as the database provider
        optionsBuilder.UseSqlServer(
            connectionString,
            x => x.MigrationsHistoryTable("__EFMigrationsHistory__Membership","membership"));

        return new MembershipDbContext(optionsBuilder.Options);
    }
}