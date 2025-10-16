namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;

/// <summary>
/// Request status data transfer object
/// </summary>
public record RequestStatusDto
{
  /// <summary>
  /// Request status
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Human-readable status description
  /// </summary>
  public string StatusDescription { get; init; } = null!;

  /// <summary>
  /// Request description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Rejection reason (if rejected)
  /// </summary>
  public string? RejectionReason { get; init; }

  /// <summary>
  /// Indicates if status is terminal
  /// </summary>
  public bool IsTerminal { get; init; }

  /// <summary>
  /// Indicates if request can be cancelled
  /// </summary>
  public bool CanBeCancelled { get; init; }

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
  /// Indicates if request requires applicant action
  /// </summary>
  public bool RequiresApplicantAction { get; init; }

  /// <summary>
  /// Indicates if request requires bank action
  /// </summary>
  public bool RequiresBankAction { get; init; }
}