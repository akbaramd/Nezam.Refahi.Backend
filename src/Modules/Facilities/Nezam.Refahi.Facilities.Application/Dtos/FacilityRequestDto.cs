namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Simple facility request data transfer object for lists
/// Contains only essential fields for list/array display
/// </summary>
public class FacilityRequestDto
{
  /// <summary>
  /// Unique request identifier
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Associated facility information
  /// </summary>
  public FacilityInfoDto Facility { get; set; } = null!;

  /// <summary>
  /// Associated cycle information
  /// </summary>
  public FacilityCycleWithUserDto Cycle { get; set; } = null!;

  /// <summary>
  /// Applicant information
  /// </summary>
  public ApplicantInfoDto Applicant { get; set; } = null!;

  /// <summary>
  /// Requested amount in Rials
  /// </summary>
  public decimal RequestedAmountRials { get; set; }

  /// <summary>
  /// Approved amount in Rials
  /// </summary>
  public decimal? ApprovedAmountRials { get; set; }

  /// <summary>
  /// Currency code
  /// </summary>
  public string Currency { get; set; } = "IRR";

  /// <summary>
  /// Request status
  /// </summary>
  public string Status { get; set; } = null!;

  /// <summary>
  /// Human-readable status text
  /// </summary>
  public string StatusText { get; set; } = null!;

  /// <summary>
  /// Request creation timestamp
  /// </summary>
  public DateTime CreatedAt { get; set; }

  public DateTime? ApprovedAt { get;  set; }
  public DateTime? RejectedAt { get;  set; }
  public string? RejectionReason { get;  set; }
  
  /// <summary>
  /// Days since request was created
  /// </summary>
  public int DaysSinceCreated { get; set; }

  /// <summary>
  /// Indicates if request is in progress
  /// </summary>
  public bool IsInProgress { get; set; }

  /// <summary>
  /// Indicates if request is completed
  /// </summary>
  public bool IsCompleted { get; set; }

  /// <summary>
  /// Indicates if request is rejected
  /// </summary>
  public bool IsRejected { get; set; }

  /// <summary>
  /// Indicates if request is cancelled
  /// </summary>
  public bool IsCancelled { get; set; }
}