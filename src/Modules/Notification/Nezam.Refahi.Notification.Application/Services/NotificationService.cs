using MCA.SharedKernel.Domain.Models;
using MediatR;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkAsRead;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUnreadCount;
using Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUserNotifications;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Services;

/// <summary>
/// Application service implementation for notification operations
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IMediator _mediator;
    
    public NotificationService(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    
    public async Task<ApplicationResult<CreateNotificationResponse>> CreateNotificationAsync(CreateNotificationCommand command)
    {
        return await _mediator.Send(command);
    }
    
    public async Task<ApplicationResult<PaginatedResult<NotificationDto>>> GetUserNotificationsAsync(GetUserNotificationsQuery query)
    {
        return await _mediator.Send(query);
    }
    
    public async Task<ApplicationResult<UnreadCountResponse>> GetUnreadCountAsync(GetUnreadCountQuery query)
    {
        return await _mediator.Send(query);
    }
    
    public async Task<ApplicationResult> MarkAsReadAsync(MarkAsReadCommand command)
    {
        return await _mediator.Send(command);
    }
}
