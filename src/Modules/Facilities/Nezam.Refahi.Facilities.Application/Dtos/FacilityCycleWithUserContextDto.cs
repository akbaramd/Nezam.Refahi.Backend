namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Facility cycle with user context data transfer object
/// </summary>
public class FacilityCycleWithUserDto
{
  /// <summary>
  /// Cycle ID
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Cycle name
  /// </summary>
  public string Name { get; set; } = null!;

  /// <summary>
  /// Cycle start date
  /// </summary>
  public DateTime StartDate { get; set; }

  /// <summary>
  /// Cycle end date
  /// </summary>
  public DateTime EndDate { get; set; }

  /// <summary>
  /// Days remaining until cycle starts
  /// </summary>
  public int DaysUntilStart { get; set; }

  /// <summary>
  /// Days remaining until cycle ends
  /// </summary>
  public int DaysUntilEnd { get; set; }

  /// <summary>
  /// Indicates if cycle has started
  /// </summary>
  public bool HasStarted { get; set; }

  /// <summary>
  /// Indicates if cycle has ended
  /// </summary>
  public bool HasEnded { get; set; }

  /// <summary>
  /// Indicates if cycle is currently active
  /// </summary>
  public bool IsActive { get; set; }

  /// <summary>
  /// Indicates if cycle is accepting applications
  /// </summary>
  public bool IsAcceptingApplications { get; set; }

  /// <summary>
  /// Total quota for this cycle
  /// </summary>
  public int Quota { get; set; }

  /// <summary>
  /// Used quota count
  /// </summary>
  public int UsedQuota { get; set; }

  /// <summary>
  /// Available quota count
  /// </summary>
  public int AvailableQuota { get; set; }

  /// <summary>
  /// Quota utilization percentage
  /// </summary>
  public decimal QuotaUtilizationPercentage { get; set; }

  /// <summary>
  /// Cycle status
  /// </summary>
  public string Status { get; set; } = null!;

  /// <summary>
  /// Human-readable status description
  /// </summary>
  public string StatusText { get; set; } = null!;

  /// <summary>
  /// Cycle description
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Financial terms for this cycle
  /// </summary>
  public FinancialTermsDto FinancialTerms { get; set; } = null!;

  /// <summary>
  /// Cycle rules and policies
  /// </summary>
  public CycleRulesDto Rules { get; set; } = null!;

  /// <summary>
  /// User eligibility information (if NationalNumber provided)
  /// </summary>
  public UserEligibilityDto? UserEligibility { get; set; }

  /// <summary>
  /// User request history for this cycle (if NationalNumber provided)
  /// </summary>
  /// </summary>
  public FacilityRequestDto? LastRequest { get; set; }

  /// <summary>
  /// Cycle creation timestamp
  /// </summary>
  public DateTime CreatedAt { get; set; }
}