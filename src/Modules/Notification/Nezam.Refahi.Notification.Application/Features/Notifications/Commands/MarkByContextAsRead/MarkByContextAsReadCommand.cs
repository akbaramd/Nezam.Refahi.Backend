using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Application.Services;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Commands.MarkByContextAsRead;

/// <summary>
/// Command to mark notifications by context as read for a user
/// </summary>
public class MarkByContextAsReadCommand : IRequest<ApplicationResult>
{
    public Guid ExternalUserId { get; set; }
    public string Context { get; set; } = string.Empty;
}

/// <summary>
/// Handler for mark by context as read command
/// </summary>
public class MarkByContextAsReadCommandHandler : IRequestHandler<MarkByContextAsReadCommand, ApplicationResult>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationUnitOfWork _unitOfWork;
    private readonly ILogger<MarkByContextAsReadCommandHandler> _logger;

    public MarkByContextAsReadCommandHandler(
        INotificationRepository notificationRepository,
        INotificationUnitOfWork unitOfWork,
        ILogger<MarkByContextAsReadCommandHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult> Handle(MarkByContextAsReadCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Marking notifications by context '{Context}' as read for user {ExternalUserId}", 
                request.Context, request.ExternalUserId);

            await _notificationRepository.MarkAsReadByContextAsync(request.ExternalUserId, request.Context, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully marked notifications by context '{Context}' as read for user {ExternalUserId}", 
                request.Context, request.ExternalUserId);

            return ApplicationResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to mark notifications by context '{Context}' as read for user {ExternalUserId}", 
                request.Context, request.ExternalUserId);
            return ApplicationResult.Failure(ex, "Failed to mark notifications by context as read");
        }
    }
}
