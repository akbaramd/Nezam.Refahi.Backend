using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Infrastructure.Persistence;

namespace Nezam.Refahi.Surveying.Infrastructure.Seeding;

/// <summary>
/// Extension methods for Survey data seeding
/// </summary>
public static class SurveySeedingExtensions
{
    /// <summary>
    /// Seeds Survey data if not already present
    /// </summary>
    public static async Task SeedSurveyDataAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<SurveyDbContext>();
            var logger = services.GetRequiredService<ILogger<SurveySeeder>>();
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            // Run seeding
            var surveyRepository = services.GetRequiredService<ISurveyRepository>();
            var unitOfWork = services.GetRequiredService<ISurveyUnitOfWork>();
            var seeder = new SurveySeeder(surveyRepository, unitOfWork, logger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<SurveySeeder>>();
            logger.LogError(ex, "An error occurred while seeding Survey data");
            throw;
        }
    }

    /// <summary>
    /// Seeds Survey data during application startup
    /// </summary>
    public static async Task SeedSurveyDataOnStartupAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<SurveyDbContext>();
            var logger = services.GetRequiredService<ILogger<SurveySeeder>>();
            
            // Check if database exists
            if (!await context.Database.CanConnectAsync())
            {
                logger.LogInformation("Database does not exist. Creating database...");
                await context.Database.EnsureCreatedAsync();
            }
            
            // Run seeding
            var surveyRepository = services.GetRequiredService<ISurveyRepository>();
            var unitOfWork = services.GetRequiredService<ISurveyUnitOfWork>();
            var seeder = new SurveySeeder(surveyRepository, unitOfWork, logger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<SurveySeeder>>();
            logger.LogError(ex, "An error occurred while seeding Survey data on startup");
            throw;
        }
    }
}
