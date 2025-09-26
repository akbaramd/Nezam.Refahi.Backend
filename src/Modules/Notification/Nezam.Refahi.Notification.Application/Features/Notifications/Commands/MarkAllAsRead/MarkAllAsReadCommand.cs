using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkAllAsRead;

/// <summary>
/// Command to mark all notifications as read for a user
/// </summary>
public class MarkAllAsReadCommand : IRequest<ApplicationResult>
{
    public Guid ExternalUserId { get; set; }
}

/// <summary>
/// Handler for mark all as read command
/// </summary>
public class MarkAllAsReadCommandHandler : IRequestHandler<MarkAllAsReadCommand, ApplicationResult>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationUnitOfWork _unitOfWork;
    private readonly ILogger<MarkAllAsReadCommandHandler> _logger;

    public MarkAllAsReadCommandHandler(
        INotificationRepository notificationRepository,
        INotificationUnitOfWork unitOfWork,
        ILogger<MarkAllAsReadCommandHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult> Handle(MarkAllAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Marking all notifications as read for user {ExternalUserId}", request.ExternalUserId);

            await _notificationRepository.MarkAllAsReadAsync(request.ExternalUserId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully marked all notifications as read for user {ExternalUserId}", request.ExternalUserId);

            return ApplicationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark all notifications as read for user {ExternalUserId}", request.ExternalUserId);
            return ApplicationResult.Failure(ex, "Failed to mark all notifications as read");
        }
    }
}
