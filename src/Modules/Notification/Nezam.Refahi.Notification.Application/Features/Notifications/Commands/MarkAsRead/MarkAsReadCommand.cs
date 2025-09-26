using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkAsRead;

/// <summary>
/// Command to mark a notification as read
/// </summary>
public class MarkAsReadCommand : IRequest<ApplicationResult>
{
    public Guid NotificationId { get; set; }
    public Guid ExternalUserId { get; set; }
}
