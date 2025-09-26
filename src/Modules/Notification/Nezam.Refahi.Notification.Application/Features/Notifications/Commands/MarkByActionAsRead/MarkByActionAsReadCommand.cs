using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkByActionAsRead;

/// <summary>
/// Command to mark notifications by action as read for a user
/// </summary>
public class MarkByActionAsReadCommand : IRequest<ApplicationResult>
{
    public Guid ExternalUserId { get; set; }
    public string Action { get; set; } = string.Empty;
}

/// <summary>
/// Handler for mark by action as read command
/// </summary>
public class MarkByActionAsReadCommandHandler : IRequestHandler<MarkByActionAsReadCommand, ApplicationResult>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationUnitOfWork _unitOfWork;
    private readonly ILogger<MarkByActionAsReadCommandHandler> _logger;

    public MarkByActionAsReadCommandHandler(
        INotificationRepository notificationRepository,
        INotificationUnitOfWork unitOfWork,
        ILogger<MarkByActionAsReadCommandHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult> Handle(MarkByActionAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Marking notifications by action '{Action}' as read for user {ExternalUserId}", 
                request.Action, request.ExternalUserId);

            await _notificationRepository.MarkAsReadByActionAsync(request.ExternalUserId, request.Action, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully marked notifications by action '{Action}' as read for user {ExternalUserId}", 
                request.Action, request.ExternalUserId);

            return ApplicationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark notifications by action '{Action}' as read for user {ExternalUserId}", 
                request.Action, request.ExternalUserId);
            return ApplicationResult.Failure(ex, "Failed to mark notifications by action as read");
        }
    }
}
