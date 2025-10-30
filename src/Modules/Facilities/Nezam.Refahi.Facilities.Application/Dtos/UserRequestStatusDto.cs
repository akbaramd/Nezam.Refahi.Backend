namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// User request status data transfer object
/// </summary>
public record UserRequestStatusDto
{
  /// <summary>
  /// Indicates if user has requested this cycle
  /// </summary>
  public bool HasRequested { get; init; }

  /// <summary>
  /// Request ID (if exists)
  /// </summary>
  public Guid? RequestId { get; init; }

  /// <summary>
  /// Request status (if exists)
  /// </summary>
  public string? RequestStatus { get; init; }

  /// <summary>
  /// Human-readable request status description (if exists)
  /// </summary>
  public string? RequestStatusDescription { get; init; }

  /// <summary>
  /// Request creation date (if exists)
  /// </summary>
  public DateTime? RequestCreatedAt { get; init; }

  /// <summary>
  /// Requested amount (if exists)
  /// </summary>
  public decimal? RequestedAmountRials { get; init; }

  /// <summary>
  /// Indicates if request is in progress
  /// </summary>
  public bool IsRequestInProgress { get; init; }

  /// <summary>
  /// Indicates if request is completed
  /// </summary>
  public bool IsRequestCompleted { get; init; }

  /// <summary>
  /// Indicates if request is rejected
  /// </summary>
  public bool IsRequestRejected { get; init; }

  /// <summary>
  /// Indicates if request is cancelled
  /// </summary>
  public bool IsRequestCancelled { get; init; }

  /// <summary>
  /// Days since request was created
  /// </summary>
  public int? DaysSinceRequestCreated { get; init; }

  /// <summary>
  /// Request approval date (if exists)
  /// </summary>
  public DateTime? RequestApprovedAt { get; init; }

  /// <summary>
  /// Request rejection date (if exists)
  /// </summary>
  public DateTime? RequestRejectedAt { get; init; }

  /// <summary>
  /// Request rejection reason (if exists)
  /// </summary>
  public string? RequestRejectionReason { get; init; }

  /// <summary>
  /// Approved amount in Rials (if exists)
  /// </summary>
  public decimal? ApprovedAmountRials { get; init; }

  /// <summary>
  /// Formatted requested amount
  /// </summary>
  public string? FormattedRequestedAmount { get; init; }

  /// <summary>
  /// Formatted approved amount
  /// </summary>
  public string? FormattedApprovedAmount { get; init; }

  /// <summary>
  /// Request processing timeline information
  /// </summary>
  public RequestTimelineDto? RequestTimeline { get; init; }
}