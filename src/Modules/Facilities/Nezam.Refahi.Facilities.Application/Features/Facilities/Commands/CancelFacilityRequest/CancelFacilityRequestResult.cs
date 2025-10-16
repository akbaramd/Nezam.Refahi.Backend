namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CancelFacilityRequest;

/// <summary>
/// Response for facility request cancellation
/// </summary>
public record CancelFacilityRequestResult
{
  /// <summary>
  /// Cancelled facility request ID
  /// </summary>
  public Guid RequestId { get; init; }

  /// <summary>
  /// Request number
  /// </summary>
  public string RequestNumber { get; init; } = null!;

  /// <summary>
  /// New status after cancellation
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Cancellation reason (if provided)
  /// </summary>
  public string? Reason { get; init; }

  /// <summary>
  /// Cancellation timestamp
  /// </summary>
  public DateTime CancelledAt { get; init; }

  /// <summary>
  /// User ID who cancelled the request
  /// </summary>
  public Guid CancelledByUserId { get; init; }
}