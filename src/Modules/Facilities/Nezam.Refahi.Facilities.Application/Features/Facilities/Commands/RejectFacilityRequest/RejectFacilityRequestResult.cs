namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.RejectFacilityRequest;

/// <summary>
/// Response for facility request rejection
/// </summary>
public record RejectFacilityRequestResult
{
  /// <summary>
  /// Rejected facility request ID
  /// </summary>
  public Guid RequestId { get; init; }

  /// <summary>
  /// Request number
  /// </summary>
  public string RequestNumber { get; init; } = null!;

  /// <summary>
  /// New status after rejection
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Rejection reason
  /// </summary>
  public string Reason { get; init; } = null!;

  /// <summary>
  /// Rejection timestamp
  /// </summary>
  public DateTime RejectedAt { get; init; }

  /// <summary>
  /// Rejector user ID
  /// </summary>
  public Guid RejectorUserId { get; init; }
}