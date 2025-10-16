using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;

/// <summary>
/// Simple facility request data transfer object for lists
/// Contains only essential fields for list/array display
/// </summary>
public record FacilityRequestDto
{
  /// <summary>
  /// Unique request identifier
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Associated facility information
  /// </summary>
  public FacilityInfoDto Facility { get; init; } = null!;

  /// <summary>
  /// Associated cycle information
  /// </summary>
  public FacilityCycleDto Cycle { get; init; } = null!;

  /// <summary>
  /// Applicant information
  /// </summary>
  public ApplicantInfoDto Applicant { get; init; } = null!;

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
  /// Request status
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Human-readable status text
  /// </summary>
  public string StatusText { get; init; } = null!;

  /// <summary>
  /// Request creation timestamp
  /// </summary>
  public DateTime CreatedAt { get; init; }

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
}