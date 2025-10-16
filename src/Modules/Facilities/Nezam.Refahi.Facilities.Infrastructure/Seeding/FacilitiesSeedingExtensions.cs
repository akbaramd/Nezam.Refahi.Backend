using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Infrastructure.Persistence;

namespace Nezam.Refahi.Facilities.Infrastructure.Seeding;

/// <summary>
/// Extension methods for Facilities data seeding
/// </summary>
public static class FacilitiesSeedingExtensions
{
    /// <summary>
    /// Seeds Facilities data if not already present
    /// </summary>
    public static async Task SeedFacilitiesDataAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<FacilitiesDbContext>();
            var logger = services.GetRequiredService<ILogger<FacilitiesSeeder>>();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Run seeding
            var facilityRepository = services.GetRequiredService<IFacilityRepository>();
            var facilityCycleRepository = services.GetRequiredService<IFacilityCycleRepository>();
            var unitOfWork = services.GetRequiredService<IFacilitiesUnitOfWork>();
            var seeder = new FacilitiesSeeder(facilityRepository, facilityCycleRepository, unitOfWork, logger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<FacilitiesSeeder>>();
            logger.LogError(ex, "An error occurred while seeding Facilities data");
            throw;
        }
    }

    /// <summary>
    /// Seeds Facilities data during application startup
    /// </summary>
    public static async Task SeedFacilitiesDataOnStartupAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<FacilitiesDbContext>();
            var logger = services.GetRequiredService<ILogger<FacilitiesSeeder>>();
            
            // Check if database exists
            if (!await context.Database.CanConnectAsync())
            {
                logger.LogInformation("Database does not exist. Creating database...");
                await context.Database.EnsureCreatedAsync();
            }
            
            // Run seeding
            var facilityRepository = services.GetRequiredService<IFacilityRepository>();
            var facilityCycleRepository = services.GetRequiredService<IFacilityCycleRepository>();
            var unitOfWork = services.GetRequiredService<IFacilitiesUnitOfWork>();
            var seeder = new FacilitiesSeeder(facilityRepository, facilityCycleRepository, unitOfWork, logger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<FacilitiesSeeder>>();
            logger.LogError(ex, "An error occurred while seeding Facilities data on startup");
            throw;
        }
    }
}
