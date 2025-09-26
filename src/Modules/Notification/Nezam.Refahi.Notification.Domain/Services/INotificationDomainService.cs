namespace Nezam.Refahi.Notifications.Domain.Services;

/// <summary>
/// Domain service for notification business logic
/// </summary>
public interface INotificationDomainService
{
    /// <summary>
    /// Creates a notification with business rules validation
    /// </summary>
    Task<Entities.Notification> CreateNotificationAsync(
        Guid externalUserId,
        string title,
        string message,
        string context,
        string action,
        string? data = null,
        DateTime? expiresAt = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates if a user can receive notifications
    /// </summary>
    Task<bool> CanUserReceiveNotificationAsync(Guid externalUserId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets notification statistics for a user
    /// </summary>
    Task<NotificationStats> GetUserNotificationStatsAsync(Guid externalUserId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Notification statistics for a user
/// </summary>
public class NotificationStats
{
    public int TotalNotifications { get; set; }
    public int UnreadNotifications { get; set; }
    public int ExpiredNotifications { get; set; }
    public DateTime? LastNotificationDate { get; set; }
}