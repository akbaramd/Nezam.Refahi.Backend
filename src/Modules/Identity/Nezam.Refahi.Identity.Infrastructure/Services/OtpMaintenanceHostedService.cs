// -----------------------------------------------------------------------------
// OtpMaintenanceHostedService.cs - Background service for OTP maintenance
// -----------------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Identity.Application.Services;
using Nezam.Refahi.Identity.Application.Services.Contracts;

namespace Nezam.Refahi.Identity.Infrastructure.Services;

/// <summary>
/// Background service to run optimized OTP maintenance periodically.
/// </summary>
public class OtpMaintenanceHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OtpMaintenanceHostedService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(2);

    public OtpMaintenanceHostedService(IServiceProvider serviceProvider, ILogger<OtpMaintenanceHostedService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OTP maintenance service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformMaintenanceAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during OTP maintenance");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("OTP maintenance service stopped");
    }

    private async Task PerformMaintenanceAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var otpCleanup = scope.ServiceProvider.GetRequiredService<IOtpCleanupService>();

        try
        {
            var stats = await otpCleanup.PerformOptimizedMaintenanceAsync();
            _logger.LogInformation("OTP maintenance: expired={Expired}, consumed={Consumed}, old={Old}, total={Total}",
                stats.ExpiredChallenges, stats.ConsumedChallenges, stats.OldChallenges, stats.TotalCleaned);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to perform OTP maintenance");
        }
    }
}


