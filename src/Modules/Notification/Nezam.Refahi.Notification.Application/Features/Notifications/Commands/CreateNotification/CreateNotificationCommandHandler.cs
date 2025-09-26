using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Notifications.Domain.Services;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.CreateNotification;

/// <summary>
/// Handler for create notification command
/// </summary>
public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, ApplicationResult<CreateNotificationResponse>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationDomainService _domainService;
    private readonly INotificationUnitOfWork _unitOfWork;
    private readonly ILogger<CreateNotificationCommandHandler> _logger;
    
    public CreateNotificationCommandHandler(
        INotificationRepository notificationRepository,
        INotificationDomainService domainService,
        INotificationUnitOfWork unitOfWork,
        ILogger<CreateNotificationCommandHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ApplicationResult<CreateNotificationResponse>> Handle(
        CreateNotificationCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating notification for user {ExternalUserId}", request.ExternalUserId);
            
            // Create notification using domain service
            var notification = await _domainService.CreateNotificationAsync(
                request.ExternalUserId,
                request.Title,
                request.Message,
                request.Context,
                request.Action,
                request.Data,
                request.ExpiresAt,
                cancellationToken);
            
            // Save notification
            await _notificationRepository.AddAsync(notification, cancellationToken:cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Notification created successfully with ID {NotificationId}", notification.Id);
            
            return ApplicationResult<CreateNotificationResponse>.Success(new CreateNotificationResponse
            {
                NotificationId = notification.Id,
                CreatedAt = notification.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create notification for user {ExternalUserId}", request.ExternalUserId);
            return ApplicationResult<CreateNotificationResponse>.Failure(ex, "Failed to create notification");
        }
    }
}
