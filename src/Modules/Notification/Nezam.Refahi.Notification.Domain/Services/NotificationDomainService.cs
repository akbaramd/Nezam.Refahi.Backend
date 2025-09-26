using Nezam.Refahi.Notifications.Domain.Entities;
using Nezam.Refahi.Notifications.Domain.Repositories;

namespace Nezam.Refahi.Notifications.Domain.Services;

/// <summary>
/// Domain service implementation for notification business logic
/// </summary>
public class NotificationDomainService : INotificationDomainService
{
    private readonly INotificationRepository _notificationRepository;
    
    public NotificationDomainService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
    }
    
    public async Task<Notification> CreateNotificationAsync(
        Guid externalUserId,
        string title,
        string message,
        string context,
        string action,
        string? data = null,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default)
    {
        // Validate user can receive notifications
        if (!await CanUserReceiveNotificationAsync(externalUserId, cancellationToken))
        {
            throw new InvalidOperationException("User cannot receive notifications");
        }
        
        // Create notification
        var notification = new Entities.Notification(
            externalUserId,
            title,
            message,
            context,
            action,
            data,
            expiresAt
        );
        
        return notification;
    }
    
    public async Task<bool> CanUserReceiveNotificationAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        // Basic validation - in a real system, you might check user preferences, status, etc.
        if (externalUserId == Guid.Empty)
            return false;
            
        // Check if user has too many unread notifications (rate limiting)
        var unreadCount = await _notificationRepository.GetUnreadCountAsync(externalUserId, cancellationToken);
        if (unreadCount > 100) // Configurable limit
            return false;
            
        return true;
    }
    
    public async Task<NotificationStats> GetUserNotificationStatsAsync(Guid externalUserId, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetUserNotificationsAsync(externalUserId, 1000, 0, cancellationToken);
        
        return new NotificationStats
        {
            TotalNotifications = notifications.Count,
            UnreadNotifications = notifications.Count(n => !n.IsRead),
            ExpiredNotifications = notifications.Count(n => n.IsExpired),
            LastNotificationDate = notifications.OrderByDescending(n => n.CreatedAt).FirstOrDefault()?.CreatedAt
        };
    }
}