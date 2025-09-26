using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Notifications.Application.Features.Notifications.Queries.GetUnreadCount;

/// <summary>
/// Query to get unread notifications count
/// </summary>
public class GetUnreadCountQuery : IRequest<ApplicationResult<UnreadCountResponse>>
{
    public Guid ExternalUserId { get; set; }
    public bool IncludeContextBreakdown { get; set; } = false;
    public bool IncludeActionBreakdown { get; set; } = false;
}

/// <summary>
/// Response for get unread count query
/// </summary>
public class UnreadCountResponse
{
    public int TotalCount { get; set; }
    public Dictionary<string, int>? ContextBreakdown { get; set; }
    public Dictionary<string, int>? ActionBreakdown { get; set; }
}
