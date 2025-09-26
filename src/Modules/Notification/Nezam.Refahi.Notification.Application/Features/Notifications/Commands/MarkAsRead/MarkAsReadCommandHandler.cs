using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkAsRead;

/// <summary>
/// Handler for mark as read command
/// </summary>
public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, ApplicationResult>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationUnitOfWork _unitOfWork;
    private readonly ILogger<MarkAsReadCommandHandler> _logger;
    
    public MarkAsReadCommandHandler(
        INotificationRepository notificationRepository,
        INotificationUnitOfWork unitOfWork,
        ILogger<MarkAsReadCommandHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ApplicationResult> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Marking notification {NotificationId} as read for user {ExternalUserId}", 
                request.NotificationId, request.ExternalUserId);
            
            // Get notification
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId, cancellationToken);
            if (notification == null)
            {
                return ApplicationResult.Failure("Notification not found");
            }
            
            // Check if user owns the notification
            if (notification.ExternalUserId != request.ExternalUserId)
            {
                return ApplicationResult.Failure("Access denied");
            }
            
            // Mark as read
            notification.MarkAsRead();
            await _notificationRepository.MarkAsReadAsync(request.NotificationId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Notification {NotificationId} marked as read successfully", request.NotificationId);
            
            return ApplicationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark notification {NotificationId} as read", request.NotificationId);
            return ApplicationResult.Failure(ex, "Failed to mark notification as read");
        }
    }
}
