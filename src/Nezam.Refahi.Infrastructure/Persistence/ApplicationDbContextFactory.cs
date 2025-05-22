using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Nezam.Refahi.Infrastructure.Persistence
{
    /// <summary>
    /// Factory for creating ApplicationDbContext instances during design-time operations
    /// such as migrations. This follows the DDD principle of keeping infrastructure concerns
    /// separate from domain logic.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
      

            // Get the connection string from configuration or use a default SQLite connection
            var connectionString = 
              "Data Source=nezam_refahi.db";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            // Use SQLite as the database provider
            optionsBuilder.UseSqlite(
                connectionString,
                x => x.MigrationsAssembly("Nezam.Refahi.Infrastructure"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
