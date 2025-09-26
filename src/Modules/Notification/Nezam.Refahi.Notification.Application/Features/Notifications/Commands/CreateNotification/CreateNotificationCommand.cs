using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;

/// <summary>
/// Command to create a new notification
/// </summary>
public class CreateNotificationCommand : IRequest<ApplicationResult<CreateNotificationResponse>>
{
    public Guid ExternalUserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Data { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// Response for create notification command
/// </summary>
public class CreateNotificationResponse
{
    public Guid NotificationId { get; set; }
    public DateTime CreatedAt { get; set; }
}