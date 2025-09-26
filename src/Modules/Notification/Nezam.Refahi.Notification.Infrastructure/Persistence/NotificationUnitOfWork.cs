using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Shared.Infrastructure.Persistence;

namespace Nezam.Refahi.Notifications.Infrastructure.Persistence;

/// <summary>
/// Implementation of Unit of Work pattern for Notification bounded context
/// Manages database transactions and coordinates domain event publishing through MediatR
/// </summary>
public class NotificationUnitOfWork : BaseUnitOfWork<NotificationDbContext>, INotificationUnitOfWork
{
    public NotificationUnitOfWork(
        NotificationDbContext context,
        IMediator mediator,
        ILogger<NotificationUnitOfWork> logger)
        : base(context, mediator, logger)
    {
    }
}
