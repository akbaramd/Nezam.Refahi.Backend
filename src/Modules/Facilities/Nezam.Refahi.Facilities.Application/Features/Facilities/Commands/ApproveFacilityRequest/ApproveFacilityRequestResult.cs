namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.ApproveFacilityRequest;

/// <summary>
/// Response for facility request approval
/// </summary>
public record ApproveFacilityRequestResult
{
  /// <summary>
  /// Approved facility request ID
  /// </summary>
  public Guid RequestId { get; init; }

  /// <summary>
  /// Request number
  /// </summary>
  public string RequestNumber { get; init; } = null!;

  /// <summary>
  /// New status after approval
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Approved amount
  /// </summary>
  public decimal ApprovedAmountRials { get; init; }

  /// <summary>
  /// Currency
  /// </summary>
  public string Currency { get; init; } = null!;

  /// <summary>
  /// Approval timestamp
  /// </summary>
  public DateTime ApprovedAt { get; init; }

  /// <summary>
  /// Approver user ID
  /// </summary>
  public Guid ApproverUserId { get; init; }
}