using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkAsRead;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUnreadCount;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUserNotifications;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Services;

/// <summary>
/// Application service for notification operations
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Creates a new notification
    /// </summary>
    Task<ApplicationResult<CreateNotificationResponse>> CreateNotificationAsync(CreateNotificationCommand command);
    
    /// <summary>
    /// Gets user notifications
    /// </summary>
    Task<ApplicationResult<PaginatedResult<NotificationDto>>> GetUserNotificationsAsync(GetUserNotificationsQuery query);
    
    /// <summary>
    /// Gets unread notifications count
    /// </summary>
    Task<ApplicationResult<UnreadCountResponse>> GetUnreadCountAsync(GetUnreadCountQuery query);
    
    /// <summary>
    /// Marks a notification as read
    /// </summary>
    Task<ApplicationResult> MarkAsReadAsync(MarkAsReadCommand command);
}
