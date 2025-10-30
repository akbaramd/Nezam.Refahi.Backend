namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// User request history data transfer object
/// </summary>
public record UserRequestHistoryDto
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
  /// Human-readable status text
  /// </summary>
  public string StatusText { get; init; } = null!;

  /// <summary>
  /// Requested amount in Rials
  /// </summary>
  public decimal RequestedAmountRials { get; init; }

  /// <summary>
  /// Approved amount in Rials (if different)
  /// </summary>
  public decimal? ApprovedAmountRials { get; init; }

  /// <summary>
  /// Request creation date
  /// </summary>
  public DateTime CreatedAt { get; init; }

  /// <summary>
  /// Request approval date (if approved)
  /// </summary>
  public DateTime? ApprovedAt { get; init; }

  /// <summary>
  /// Request rejection date (if rejected)
  /// </summary>
  public DateTime? RejectedAt { get; init; }

  /// <summary>
  /// Rejection reason (if rejected)
  /// </summary>
  public string? RejectionReason { get; init; }

  /// <summary>
  /// Days since request creation
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
}