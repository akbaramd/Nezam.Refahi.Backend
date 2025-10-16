namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CreateFacilityRequest;

/// <summary>
/// Response for facility request creation
/// </summary>
public record CreateFacilityRequestResult
{
  /// <summary>
  /// Created facility request ID
  /// </summary>
  public Guid RequestId { get; init; }

  /// <summary>
  /// Generated request number
  /// </summary>
  public string RequestNumber { get; init; } = null!;

  /// <summary>
  /// Request status
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Requested amount
  /// </summary>
  public decimal RequestedAmountRials { get; init; }

  /// <summary>
  /// Currency
  /// </summary>
  public string Currency { get; init; } = null!;

  /// <summary>
  /// Creation timestamp
  /// </summary>
  public DateTime CreatedAt { get; init; }
}