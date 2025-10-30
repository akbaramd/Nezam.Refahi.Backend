using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services.Contracts;
using Nezam.Refahi.Identity.Infrastructure.ACL.Contracts;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Identity.Domain.Entities;
using Nezam.Refahi.Identity.Domain.ValueObjects;
using Nezam.Refahi.Identity.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Hosted service for automatic user seeding operations
/// Runs periodically to sync users from external sources
/// </summary>
public class UserSeedingHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserSeedingHostedService> _logger;
    private readonly TimeSpan _period = TimeSpan.FromHours(6); // Run every 6 hours
    private static bool _initialSeedingCompleted = false;
    private static readonly object _lockObject = new object();

    public UserSeedingHostedService(
        IServiceProvider serviceProvider,
        ILogger<UserSeedingHostedService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("User Seeding Hosted Service started");

        // Perform initial seeding on first start
        lock (_lockObject)
        {
            if (!_initialSeedingCompleted)
            {
                try
                {
                    _logger.LogInformation("Performing initial administrator user seeding");
                    PerformInitialSeedingAsync(stoppingToken).Wait(stoppingToken);
                    _initialSeedingCompleted = true;
                    _logger.LogInformation("Initial administrator user seeding completed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during initial administrator user seeding");
                }
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformSeedingAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user seeding operation");
            }

            await Task.Delay(_period, stoppingToken);
        }

        _logger.LogInformation("User Seeding Hosted Service stopped");
    }

    /// <summary>
    /// Performs initial seeding of administrator user
    /// </summary>
    private async Task PerformInitialSeedingAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();

        try
        {
            // Check if administrator user already exists
            var phoneNumber = new PhoneNumber("09371770774");
            var existingAdmin = await userRepository.GetByPhoneNumberValueObjectAsync(phoneNumber);
            
            if (existingAdmin != null)
            {
                _logger.LogInformation("Administrator user already exists, skipping initial seeding");
                return;
            }

            _logger.LogInformation("Creating administrator user: akbar admin");

            // Create administrator user with full constructor
            var adminUser = new User("akbar", "admin", "2741153671", "09371770774");
            adminUser.VerifyPhone();

            // Save the user first
            await userRepository.AddAsync(adminUser, true);

            // Create or get Administrator role
            var adminRole = await roleRepository.GetByNameAsync("Administrator");
            if (adminRole == null)
            {
                adminRole = new Role("Administrator", "System Administrator with full access", true);
                await roleRepository.AddAsync(adminRole, true);
                _logger.LogInformation("Created Administrator role");
            }

            // Assign Administrator role to the user
            adminUser.AssignRole(adminRole.Id);
            await userRepository.UpdateAsync(adminUser, true);

            _logger.LogInformation("Successfully created administrator user: {UserId} with phone: {PhoneNumber} and national code: {NationalCode}", 
                adminUser.Id, "09371770774", "2741153671");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating administrator user during initial seeding");
            throw;
        }
    }

    private async Task PerformSeedingAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var orchestrator = scope.ServiceProvider.GetRequiredService<IUserSeedOrchestrator>();

        _logger.LogInformation("Starting scheduled user seeding operation");

        try
        {
            // First validate configuration
            var validationResult = await orchestrator.ValidateConfigurationAsync(cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Seeding configuration validation failed: {Errors}", 
                    string.Join(", ", validationResult.Errors));
                return;
            }

            if (validationResult.Warnings.Count > 0)
            {
                _logger.LogWarning("Seeding configuration warnings: {Warnings}", 
                    string.Join(", ", validationResult.Warnings));
            }

            // Run incremental seeding (this would use stored watermark)
            var result = await orchestrator.RunIncrementalAsync(
                watermark: new UserSeedWatermark(), // In real implementation, load from storage
                batchSize: 1000,
                maxParallel: 4,
                dryRun: false,
                cancellationToken: cancellationToken);

            _logger.LogInformation("User seeding operation completed: " +
                "Processed: {TotalProcessed}, Created: {SuccessfullyCreated}, " +
                "Skipped: {Skipped}, Failed: {Failed}, Duration: {Duration}",
                result.TotalProcessed, result.SuccessfullyCreated, 
                result.Skipped, result.Failed, result.Duration);

            if (result.Errors.Count > 0)
            {
                _logger.LogWarning("Seeding operation had {ErrorCount} errors", result.Errors.Count);
                foreach (var error in result.Errors.Take(5)) // Log first 5 errors
                {
                    _logger.LogWarning("Seeding error: {ErrorCode} - {Message}", 
                        error.ErrorCode, error.Message);
                }
            }

            if (result.Warnings.Count > 0)
            {
                _logger.LogInformation("Seeding operation had {WarningCount} warnings", result.Warnings.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error during user seeding operation");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("User Seeding Hosted Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}
