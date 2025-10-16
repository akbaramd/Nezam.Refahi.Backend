using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Surveying.Infrastructure.Persistence;

namespace Nezam.Refahi.Surveying.Infrastructure.Seeding;

/// <summary>
/// Hosted service to seed Survey data on application startup
/// </summary>
public class SurveySeedingHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SurveySeedingHostedService> _logger;

    public SurveySeedingHostedService(IServiceProvider serviceProvider, ILogger<SurveySeedingHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting Survey data seeding...");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SurveyDbContext>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // Check if data already exists
            if (await context.Surveys.AnyAsync(cancellationToken))
            {
                _logger.LogInformation("Survey data already exists. Skipping seeding.");
                return;
            }

            // Run seeding
            var surveyRepository = scope.ServiceProvider.GetRequiredService<ISurveyRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<ISurveyUnitOfWork>();
            var seederLogger = scope.ServiceProvider.GetRequiredService<ILogger<SurveySeeder>>();
            var seeder = new SurveySeeder(surveyRepository, unitOfWork, seederLogger);
            await seeder.SeedAsync();

            _logger.LogInformation("Survey data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Survey data seeding");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
