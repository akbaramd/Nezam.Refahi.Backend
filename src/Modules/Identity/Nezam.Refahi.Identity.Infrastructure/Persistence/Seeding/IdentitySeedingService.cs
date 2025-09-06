using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nezam.Refahi.Identity.Infrastructure.Persistence.Seeding;

/// <summary>
/// Hosted service that automatically seeds Identity data on application startup
/// </summary>
public class IdentitySeedingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IdentitySeedingService> _logger;

    public IdentitySeedingService(IServiceProvider serviceProvider, ILogger<IdentitySeedingService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Identity seeding service started");

        try
        {
            // Wait a bit to ensure the application is fully started
            await Task.Delay(2000, stoppingToken);

            // Create a new scope for seeding operations
            using var scope = _serviceProvider.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<IdentityDataSeeder>();

            _logger.LogInformation("Checking if Identity data seeding is needed...");

            // Check if seeding is needed
            var needsSeeding = await seeder.IsSeedingNeededAsync();
            if (!needsSeeding)
            {
                _logger.LogInformation("Identity data seeding is not needed, all data already exists");
                return;
            }

            _logger.LogInformation("Identity data seeding is needed, starting seeding process...");

            // Perform seeding
            await seeder.SeedAllDataAsync();

            _logger.LogInformation("Identity data seeding completed, validating results...");

            // Validate seeding results
            var validation = await seeder.ValidateSeedingAsync();
            if (validation.IsValid)
            {
                _logger.LogInformation("Identity data seeding completed successfully. Roles: {RoleCount}, Admin Users: {AdminUserCount}", 
                    validation.RoleCount, validation.AdminUserCount);
            }
            else
            {
                _logger.LogWarning("Identity data seeding completed with issues. Validation: {ValidationError}", 
                    validation.ValidationError ?? "Unknown validation error");
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Identity seeding service was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed Identity data during application startup");
            // Don't rethrow - we don't want to crash the application if seeding fails
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Identity seeding service stopped");
        await base.StopAsync(cancellationToken);
    }
}
