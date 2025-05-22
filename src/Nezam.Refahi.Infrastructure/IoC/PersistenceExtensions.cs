using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Application.Common.Interfaces;
using Nezam.Refahi.Infrastructure.Persistence;

namespace Nezam.Refahi.Infrastructure.IoC;

/// <summary>
/// Extension methods for registering persistence services in the DI container
/// </summary>
public static class PersistenceExtensions
{
    /// <summary>
    /// Adds database context and related services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the database context
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("SqliteConnection");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Database connection string 'DefaultConnection' is not configured.");
            }
            
            options.UseSqlite(connectionString);
        });
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        return services;
    }
} 
