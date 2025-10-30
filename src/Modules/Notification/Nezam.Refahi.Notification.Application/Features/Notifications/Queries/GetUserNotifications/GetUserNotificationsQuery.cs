using MCA.SharedKernel.Domain.Models;
using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUserNotifications;

/// <summary>
/// Query to get user notifications
/// </summary>
public class GetUserNotificationsQuery : IRequest<ApplicationResult<PaginatedResult<NotificationDto>>>
{
    public Guid ExternalUserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool? IsRead { get; set; }
    public string? Context { get; set; }
    public string? Action { get; set; }
}

/// <summary>
/// Response for get user notifications query
/// </summary>
public class GetUserNotificationsResponse
{
    public List<NotificationDto> Notifications { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}

/// <summary>
/// Notification DTO
/// </summary>
public class NotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public dynamic? Data { get; set; }
    public bool HasAction { get; set; }
    public bool IsExpired { get; set; }
}
