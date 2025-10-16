using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserMemberInfo;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCyclesWithRequests;

/// <summary>
/// Response for user cycles with requests query
/// </summary>
public record GetUserCyclesWithRequestsResponse
{
    /// <summary>
    /// User member information
    /// </summary>
    public UserMemberInfoDto UserInfo { get; init; } = null!;

    /// <summary>
    /// List of cycles where user has requests
    /// </summary>
    public List<UserCycleWithRequestDto> Cycles { get; init; } = new();

    /// <summary>
    /// Total count of cycles
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Current page number
    /// </summary>
    public int Page { get; init; }

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total pages
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Summary statistics
    /// </summary>
    public UserCyclesWithRequestsSummaryDto Summary { get; init; } = null!;
}

/// <summary>
/// User cycle with request details data transfer object
/// </summary>
public record UserCycleWithRequestDto
{
    /// <summary>
    /// Cycle ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Cycle name
    /// </summary>
    public string Name { get; init; } = null!;

    /// <summary>
    /// Cycle start date
    /// </summary>
    public DateTime StartDate { get; init; }

    /// <summary>
    /// Cycle end date
    /// </summary>
    public DateTime EndDate { get; init; }

    /// <summary>
    /// Days remaining until cycle starts
    /// </summary>
    public int DaysUntilStart { get; init; }

    /// <summary>
    /// Days remaining until cycle ends
    /// </summary>
    public int DaysUntilEnd { get; init; }

    /// <summary>
    /// Indicates if cycle has started
    /// </summary>
    public bool HasStarted { get; init; }

    /// <summary>
    /// Indicates if cycle has ended
    /// </summary>
    public bool HasEnded { get; init; }

    /// <summary>
    /// Indicates if cycle is currently active
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Indicates if cycle is accepting applications
    /// </summary>
    public bool IsAcceptingApplications { get; init; }

    /// <summary>
    /// Total quota for this cycle
    /// </summary>
    public int Quota { get; init; }

    /// <summary>
    /// Used quota count
    /// </summary>
    public int UsedQuota { get; init; }

    /// <summary>
    /// Available quota count
    /// </summary>
    public int AvailableQuota { get; init; }

    /// <summary>
    /// Quota utilization percentage
    /// </summary>
    public decimal QuotaUtilizationPercentage { get; init; }

    /// <summary>
    /// Cycle status
    /// </summary>
    public string Status { get; init; } = null!;

    /// <summary>
    /// Human-readable status description
    /// </summary>
    public string StatusDescription { get; init; } = null!;

    /// <summary>
    /// Cycle description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Associated facility information
    /// </summary>
    public FacilityInfoDto? Facility { get; init; }

    /// <summary>
    /// User's request details for this cycle
    /// </summary>
    public UserRequestDetailsDto UserRequest { get; init; } = null!;

    /// <summary>
    /// Cycle creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// User request details data transfer object
/// </summary>
public record UserRequestDetailsDto
{
    /// <summary>
    /// Request ID
    /// </summary>
    public Guid RequestId { get; init; }

    /// <summary>
    /// Request status
    /// </summary>
    public string Status { get; init; } = null!;

    /// <summary>
    /// Human-readable request status description
    /// </summary>
    public string StatusDescription { get; init; } = null!;

    /// <summary>
    /// Requested amount in Rials
    /// </summary>
    public decimal RequestedAmountRials { get; init; }

    /// <summary>
    /// Approved amount in Rials
    /// </summary>
    public decimal? ApprovedAmountRials { get; init; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; init; } = "IRR";

    /// <summary>
    /// Formatted requested amount
    /// </summary>
    public string FormattedRequestedAmount { get; init; } = null!;

    /// <summary>
    /// Formatted approved amount
    /// </summary>
    public string? FormattedApprovedAmount { get; init; }

    /// <summary>
    /// Request creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Request approval timestamp
    /// </summary>
    public DateTime? ApprovedAt { get; init; }

    /// <summary>
    /// Request rejection timestamp
    /// </summary>
    public DateTime? RejectedAt { get; init; }

    /// <summary>
    /// Request rejection reason
    /// </summary>
    public string? RejectionReason { get; init; }

    /// <summary>
    /// Days since request was created
    /// </summary>
    public int DaysSinceCreated { get; init; }

    /// <summary>
    /// Indicates if request is in progress
    /// </summary>
    public bool IsInProgress { get; init; }

    /// <summary>
    /// Indicates if request is completed
    /// </summary>
    public bool IsCompleted { get; init; }

    /// <summary>
    /// Indicates if request is rejected
    /// </summary>
    public bool IsRejected { get; init; }

    /// <summary>
    /// Indicates if request is cancelled
    /// </summary>
    public bool IsCancelled { get; init; }

    /// <summary>
    /// Request timeline information
    /// </summary>
    public RequestTimelineDto? Timeline { get; init; }
}

/// <summary>
/// Summary statistics for user cycles with requests
/// </summary>
public record UserCyclesWithRequestsSummaryDto
{
    /// <summary>
    /// Total cycles where user has requests
    /// </summary>
    public int TotalCycles { get; init; }

    /// <summary>
    /// Active cycles where user has requests
    /// </summary>
    public int ActiveCycles { get; init; }

    /// <summary>
    /// Cycles with in-progress requests
    /// </summary>
    public int CyclesWithInProgressRequests { get; init; }

    /// <summary>
    /// Cycles with completed requests
    /// </summary>
    public int CyclesWithCompletedRequests { get; init; }

    /// <summary>
    /// Cycles with rejected requests
    /// </summary>
    public int CyclesWithRejectedRequests { get; init; }

    /// <summary>
    /// Total requested amount across all cycles
    /// </summary>
    public decimal TotalRequestedAmountRials { get; init; }

    /// <summary>
    /// Total approved amount across all cycles
    /// </summary>
    public decimal TotalApprovedAmountRials { get; init; }

    /// <summary>
    /// Formatted total requested amount
    /// </summary>
    public string FormattedTotalRequestedAmount { get; init; } = null!;

    /// <summary>
    /// Formatted total approved amount
    /// </summary>
    public string FormattedTotalApprovedAmount { get; init; } = null!;

    /// <summary>
    /// Different facilities count
    /// </summary>
    public int DifferentFacilitiesCount { get; init; }

    /// <summary>
    /// Most recent request date
    /// </summary>
    public DateTime? MostRecentRequestDate { get; init; }

    /// <summary>
    /// Oldest request date
    /// </summary>
    public DateTime? OldestRequestDate { get; init; }
}
