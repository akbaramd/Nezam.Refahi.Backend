using MCA.SharedKernel.Domain.Contracts.Repositories;
using Nezam.Refahi.Notifications.Domain.Entities;

namespace Nezam.Refahi.Notifications.Domain.Repositories;

/// <summary>
/// Repository interface for Notification entity
/// </summary>
public interface INotificationRepository : IRepository<Notification, Guid>
{
    /// <summary>
    /// Gets notifications for a specific user
    /// </summary>
    Task<List<Notification>> GetUserNotificationsAsync(
        Guid externalUserId, 
        int limit = 50, 
        int offset = 0,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets notifications for a specific user with context and action filtering
    /// </summary>
    Task<List<Notification>> GetUserNotificationsAsync(
        Guid externalUserId, 
        int limit = 50, 
        int offset = 0,
        string? context = null,
        string? action = null,
        bool? isRead = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets notifications by context
    /// </summary>
    Task<List<Notification>> GetNotificationsByContextAsync(
        string context,
        int limit = 50,
        int offset = 0,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets notifications by action
    /// </summary>
    Task<List<Notification>> GetNotificationsByActionAsync(
        string action,
        int limit = 50,
        int offset = 0,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets notifications by context and action
    /// </summary>
    Task<List<Notification>> GetNotificationsByContextAndActionAsync(
        string context,
        string action,
        int limit = 50,
        int offset = 0,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets unread notifications count for a user
    /// </summary>
    Task<int> GetUnreadCountAsync(Guid externalUserId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets total notifications count for a user with optional filters
    /// </summary>
    Task<int> GetTotalCountAsync(Guid externalUserId, string? context = null, string? action = null, bool? isRead = null, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets unread count by context
    /// </summary>
    Task<int> GetUnreadCountByContextAsync(
        Guid externalUserId, 
        string context, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets unread count by action
    /// </summary>
    Task<int> GetUnreadCountByActionAsync(
        Guid externalUserId, 
        string action, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets unread count by context and action
    /// </summary>
    Task<int> GetUnreadCountByContextAndActionAsync(
        Guid externalUserId, 
        string context, 
        string action, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets unread count grouped by context
    /// </summary>
    Task<Dictionary<string, int>> GetUnreadCountByContextAsync(
        Guid externalUserId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets unread count grouped by action
    /// </summary>
    Task<Dictionary<string, int>> GetUnreadCountByActionAsync(
        Guid externalUserId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Marks a notification as read
    /// </summary>
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Marks all notifications as read for a user
    /// </summary>
    Task MarkAllAsReadAsync(Guid externalUserId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Marks notifications as read by context
    /// </summary>
    Task MarkAsReadByContextAsync(
        Guid externalUserId, 
        string context, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Marks notifications as read by action
    /// </summary>
    Task MarkAsReadByActionAsync(
        Guid externalUserId, 
        string action, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes expired notifications
    /// </summary>
    Task DeleteExpiredAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes notifications by context
    /// </summary>
    Task DeleteByContextAsync(
        Guid externalUserId, 
        string context, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes notifications by action
    /// </summary>
    Task DeleteByActionAsync(
        Guid externalUserId, 
        string action, 
        CancellationToken cancellationToken = default);
}