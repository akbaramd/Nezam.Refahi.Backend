using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

/// <summary>
/// Facility cycle with user context data transfer object
/// </summary>
public record FacilityCycleWithUserContextDto
{
  /// <summary>
  /// Cycle ID
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Cycle name
  /// </summary>
  public string Name { get; init; } = null!;

  /// <summary>
  /// Cycle start date
  /// </summary>
  public DateTime StartDate { get; init; }

  /// <summary>
  /// Cycle end date
  /// </summary>
  public DateTime EndDate { get; init; }

  /// <summary>
  /// Days remaining until cycle starts
  /// </summary>
  public int DaysUntilStart { get; init; }

  /// <summary>
  /// Days remaining until cycle ends
  /// </summary>
  public int DaysUntilEnd { get; init; }

  /// <summary>
  /// Indicates if cycle has started
  /// </summary>
  public bool HasStarted { get; init; }

  /// <summary>
  /// Indicates if cycle has ended
  /// </summary>
  public bool HasEnded { get; init; }

  /// <summary>
  /// Indicates if cycle is currently active
  /// </summary>
  public bool IsActive { get; init; }

  /// <summary>
  /// Indicates if cycle is accepting applications
  /// </summary>
  public bool IsAcceptingApplications { get; init; }

  /// <summary>
  /// Total quota for this cycle
  /// </summary>
  public int Quota { get; init; }

  /// <summary>
  /// Used quota count
  /// </summary>
  public int UsedQuota { get; init; }

  /// <summary>
  /// Available quota count
  /// </summary>
  public int AvailableQuota { get; init; }

  /// <summary>
  /// Quota utilization percentage
  /// </summary>
  public decimal QuotaUtilizationPercentage { get; init; }

  /// <summary>
  /// Cycle status
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Human-readable status description
  /// </summary>
  public string StatusDescription { get; init; } = null!;

  /// <summary>
  /// Cycle description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Financial terms for this cycle
  /// </summary>
  public FinancialTermsDto FinancialTerms { get; init; } = null!;

  /// <summary>
  /// Cycle rules and policies
  /// </summary>
  public CycleRulesDto Rules { get; init; } = null!;

  /// <summary>
  /// User eligibility information (if NationalNumber provided)
  /// </summary>
  public UserEligibilityDto? UserEligibility { get; init; }

  /// <summary>
  /// User request history for this cycle (if NationalNumber provided)
  /// </summary>
  public List<UserRequestHistoryDto> UserRequestHistory { get; init; } = new();

  /// <summary>
  /// Last request for this cycle (if user has requests)
  /// </summary>
  public UserRequestHistoryDto? LastRequest { get; init; }

  /// <summary>
  /// Cycle creation timestamp
  /// </summary>
  public DateTime CreatedAt { get; init; }
}