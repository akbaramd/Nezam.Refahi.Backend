using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUnreadCount;

/// <summary>
/// Handler for get unread count query
/// </summary>
public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, ApplicationResult<UnreadCountResponse>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<GetUnreadCountQueryHandler> _logger;
    
    public GetUnreadCountQueryHandler(
        INotificationRepository notificationRepository,
        ILogger<GetUnreadCountQueryHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ApplicationResult<UnreadCountResponse>> Handle(
        GetUnreadCountQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting unread count for user {ExternalUserId}", request.ExternalUserId);
            
            var totalCount = await _notificationRepository.GetUnreadCountAsync(request.ExternalUserId, cancellationToken);
            
            var response = new UnreadCountResponse
            {
                TotalCount = totalCount
            };
            
            // Get context breakdown if requested
            if (request.IncludeContextBreakdown)
            {
                response.ContextBreakdown = await _notificationRepository.GetUnreadCountByContextAsync(
                    request.ExternalUserId, cancellationToken);
            }
            
            // Get action breakdown if requested
            if (request.IncludeActionBreakdown)
            {
                response.ActionBreakdown = await _notificationRepository.GetUnreadCountByActionAsync(
                    request.ExternalUserId, cancellationToken);
            }
            
            _logger.LogInformation("User {ExternalUserId} has {Count} unread notifications", 
                request.ExternalUserId, totalCount);
            
            return ApplicationResult<UnreadCountResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get unread count for user {ExternalUserId}", request.ExternalUserId);
            return ApplicationResult<UnreadCountResponse>.Failure(ex, "Failed to get unread count");
        }
    }
}
