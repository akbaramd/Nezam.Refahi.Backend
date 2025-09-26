using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Notifications.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using System.Text.Json;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUserNotifications;

/// <summary>
/// Handler for get user notifications query
/// </summary>
public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, ApplicationResult<PaginatedResult<NotificationDto>>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<GetUserNotificationsQueryHandler> _logger;
    
    public GetUserNotificationsQueryHandler(
        INotificationRepository notificationRepository,
        ILogger<GetUserNotificationsQueryHandler> logger)
    {
        _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<ApplicationResult<PaginatedResult<NotificationDto>>> Handle(
        GetUserNotificationsQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting notifications for user {ExternalUserId}, page {PageNumber}, pageSize {PageSize}", 
                request.ExternalUserId, request.PageNumber, request.PageSize);
            
            // Calculate offset
            var offset = (request.PageNumber - 1) * request.PageSize;
            
            // Get notifications with filters
            var notifications = await _notificationRepository.GetUserNotificationsAsync(
                request.ExternalUserId, 
                request.PageSize, 
                offset,
                request.Context,
                request.Action,
                request.IsRead,
                cancellationToken);
            
            // Map to DTOs
            var notificationDtos = notifications.Select(MapToDto).ToList();
            
            // Get total count for pagination
            var totalCount = await GetTotalCountAsync(request.ExternalUserId, request, cancellationToken);
            
            var paginatedResult = new PaginatedResult<NotificationDto>
            {
                Items = notificationDtos,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            
            _logger.LogInformation("Retrieved {Count} notifications for user {ExternalUserId}", 
                notificationDtos.Count, request.ExternalUserId);
            
            return ApplicationResult<PaginatedResult<NotificationDto>>.Success(paginatedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get notifications for user {ExternalUserId}", request.ExternalUserId);
            return ApplicationResult<PaginatedResult<NotificationDto>>.Failure(ex, "Failed to get notifications");
        }
    }
    
    private static List<Domain.Entities.Notification> ApplyFilters(
        List<Domain.Entities.Notification> notifications, 
        GetUserNotificationsQuery request)
    {
        var filtered = notifications.AsEnumerable();
        
        // Filter by read status
        if (request.IsRead.HasValue)
        {
            filtered = filtered.Where(n => n.IsRead == request.IsRead.Value);
        }
        
        // Filter by context
        if (!string.IsNullOrWhiteSpace(request.Context))
        {
            filtered = filtered.Where(n => n.Context.Equals(request.Context, StringComparison.OrdinalIgnoreCase));
        }
        
        // Filter by action
        if (!string.IsNullOrWhiteSpace(request.Action))
        {
            filtered = filtered.Where(n => n.Action.Equals(request.Action, StringComparison.OrdinalIgnoreCase));
        }
        
        return filtered.ToList();
    }
    
    private async Task<int> GetTotalCountAsync(
        Guid externalUserId, 
        GetUserNotificationsQuery request, 
        CancellationToken cancellationToken)
    {
        return await _notificationRepository.GetTotalCountAsync(
            externalUserId, 
            request.Context, 
            request.Action, 
            request.IsRead, 
            cancellationToken);
    }
    
    private static NotificationDto MapToDto(Domain.Entities.Notification notification)
    {
        dynamic? data = null;
        
        // Deserialize Data string to dynamic object if it exists
        if (!string.IsNullOrWhiteSpace(notification.Data))
        {
            try
            {
                data = JsonSerializer.Deserialize<dynamic>(notification.Data);
            }
            catch (JsonException)
            {
                // If deserialization fails, keep as null
                data = null;
            }
        }
        
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Context = notification.Context,
            Action = notification.Action,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt,
            ExpiresAt = notification.ExpiresAt,
            Data = data,
            HasAction = notification.HasAction,
            IsExpired = notification.IsExpired
        };
    }
}