using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Settings.Infrastructure.Persistence;

namespace Nezam.Refahi.Settings.Infrastructure.Persistence.Seeding;

/// <summary>
/// Extension methods for settings seeding
/// </summary>
public static class SettingsSeedingExtensions
{
    /// <summary>
    /// Seeds settings data if not already exists
    /// </summary>
    /// <param name="serviceProvider">Service provider</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public static async Task SeedSettingsAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SettingsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<SettingsSeedingService>>();

        try
        {
            logger.LogInformation("Starting manual settings seeding process...");

            // Ensure database is created
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // Create seeding service and run seeding
            var seedingService = new SettingsSeedingService(serviceProvider, logger);
            await seedingService.StartAsync(cancellationToken);

            logger.LogInformation("Manual settings seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during manual settings seeding");
            throw;
        }
    }

    /// <summary>
    /// Seeds settings data using DbContext directly
    /// </summary>
    /// <param name="context">Settings DbContext</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task</returns>
    public static async Task SeedSettingsAsync(this SettingsDbContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // Create seeding service and run seeding
            var serviceProvider = context.GetService<IServiceProvider>();
            var logger = serviceProvider?.GetService<ILogger<SettingsSeedingService>>() ?? 
                        throw new InvalidOperationException("Logger service not found");

            var seedingService = new SettingsSeedingService(serviceProvider, logger);
            await seedingService.StartAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error occurred during settings seeding", ex);
        }
    }

    /// <summary>
    /// Checks if settings are already seeded
    /// </summary>
    /// <param name="context">Settings DbContext</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if settings are seeded</returns>
    public static async Task<bool> IsSettingsSeededAsync(this SettingsDbContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if we have any settings sections (indicator that seeding has been done)
            var hasSections = await context.SettingsSections.AnyAsync(cancellationToken);
            var hasCategories = await context.SettingsCategories.AnyAsync(cancellationToken);
            var hasSettings = await context.Settings.AnyAsync(cancellationToken);

            return hasSections && hasCategories && hasSettings;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets seeding status information
    /// </summary>
    /// <param name="context">Settings DbContext</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Seeding status information</returns>
    public static async Task<SeedingStatus> GetSeedingStatusAsync(this SettingsDbContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var sectionsCount = await context.SettingsSections.CountAsync(cancellationToken);
            var categoriesCount = await context.SettingsCategories.CountAsync(cancellationToken);
            var settingsCount = await context.Settings.CountAsync(cancellationToken);

            return new SeedingStatus
            {
                IsSeeded = sectionsCount > 0 && categoriesCount > 0 && settingsCount > 0,
                SectionsCount = sectionsCount,
                CategoriesCount = categoriesCount,
                SettingsCount = settingsCount
            };
        }
        catch (Exception ex)
        {
            return new SeedingStatus
            {
                IsSeeded = false,
                Error = ex.Message
            };
        }
    }
}

/// <summary>
/// Seeding status information
/// </summary>
public class SeedingStatus
{
    public bool IsSeeded { get; set; }
    public int SectionsCount { get; set; }
    public int CategoriesCount { get; set; }
    public int SettingsCount { get; set; }
    public string? Error { get; set; }
}
