using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Domain.Repositories;

namespace Nezam.Refahi.Notifications.Infrastructure.BackgroundServices;

/// <summary>
/// Background service for cleaning up expired notifications
/// </summary>
public class NotificationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationCleanupService> _logger;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1); // Run every hour
    
    public NotificationCleanupService(
        IServiceProvider serviceProvider,
        ILogger<NotificationCleanupService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification cleanup service started");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupExpiredNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while cleaning up expired notifications");
            }
            
            await Task.Delay(_cleanupInterval, stoppingToken);
        }
        
        _logger.LogInformation("Notification cleanup service stopped");
    }
    
    private async Task CleanupExpiredNotificationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        
        _logger.LogDebug("Starting cleanup of expired notifications");
        
        await notificationRepository.DeleteExpiredAsync(cancellationToken);
        
        _logger.LogDebug("Completed cleanup of expired notifications");
    }
}
