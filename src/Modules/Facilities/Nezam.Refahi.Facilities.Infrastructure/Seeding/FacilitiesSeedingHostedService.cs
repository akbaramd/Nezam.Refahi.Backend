using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Facilities.Application;
using Nezam.Refahi.Facilities.Domain.Repositories;
using Nezam.Refahi.Facilities.Infrastructure.Persistence;

namespace Nezam.Refahi.Facilities.Infrastructure.Seeding;

/// <summary>
/// Hosted service to seed Facilities data on application startup
/// </summary>
public class FacilitiesSeedingHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FacilitiesSeedingHostedService> _logger;

    public FacilitiesSeedingHostedService(IServiceProvider serviceProvider, ILogger<FacilitiesSeedingHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting Facilities data seeding...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FacilitiesDbContext>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // Check if data already exists
            if (await context.Facilities.AnyAsync(cancellationToken))
            {
                _logger.LogInformation("Facilities data already exists. Skipping seeding.");
                return;
            }

            // Run seeding
            var facilityRepository = scope.ServiceProvider.GetRequiredService<IFacilityRepository>();
            var facilityCycleRepository = scope.ServiceProvider.GetRequiredService<IFacilityCycleRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IFacilitiesUnitOfWork>();
            var seederLogger = scope.ServiceProvider.GetRequiredService<ILogger<FacilitiesSeeder>>();
            var seeder = new FacilitiesSeeder(facilityRepository, facilityCycleRepository, unitOfWork, seederLogger);
            await seeder.SeedAsync();

            _logger.LogInformation("Facilities data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Facilities data seeding");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
