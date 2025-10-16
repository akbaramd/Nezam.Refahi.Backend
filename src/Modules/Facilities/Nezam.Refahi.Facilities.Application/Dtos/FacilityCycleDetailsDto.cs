using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityDetails;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Detailed facility cycle data transfer object
/// Contains full cycle information including all details
/// </summary>
public record FacilityCycleDetailsDto : FacilityCycleDto
{
  /// <summary>
  /// Days remaining until cycle starts
  /// </summary>
  public int DaysUntilStart { get; init; }

  /// <summary>
  /// Indicates if cycle has started
  /// </summary>
  public bool HasStarted { get; init; }

  /// <summary>
  /// Indicates if cycle has ended
  /// </summary>
  public bool HasEnded { get; init; }

  /// <summary>
  /// Indicates if cycle is accepting applications
  /// </summary>
  public bool IsAcceptingApplications { get; init; }

  /// <summary>
  /// Quota utilization percentage
  /// </summary>
  public decimal QuotaUtilizationPercentage { get; init; }

  /// <summary>
  /// Cycle description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Financial terms for this cycle
  /// </summary>
  public DetailedFinancialTermsDto FinancialTerms { get; init; } = null!;

  /// <summary>
  /// Cycle rules and policies
  /// </summary>
  public DetailedCycleRulesDto Rules { get; init; } = null!;

  /// <summary>
  /// Cycle dependencies
  /// </summary>
  public List<FacilityCycleDependencyDto> Dependencies { get; init; } = new();

  /// <summary>
  /// Admission strategy (FIFO, Score, Lottery)
  /// </summary>
  public string AdmissionStrategy { get; init; } = null!;

  /// <summary>
  /// Human-readable admission strategy description
  /// </summary>
  public string AdmissionStrategyDescription { get; init; } = null!;

  /// <summary>
  /// Waitlist capacity
  /// </summary>
  public int? WaitlistCapacity { get; init; }

  /// <summary>
  /// Cycle metadata
  /// </summary>
  public Dictionary<string, string> Metadata { get; init; } = new();

  /// <summary>
  /// Last modification timestamp
  /// </summary>
  public DateTime? LastModifiedAt { get; init; }

  /// <summary>
  /// Cycle statistics
  /// </summary>
  public CycleStatisticsDto Statistics { get; init; } = null!;

  /// <summary>
  /// User eligibility information (if user context provided)
  /// </summary>
  public DetailedEligibilityDto? UserEligibility { get; init; }

  /// <summary>
  /// User request history for this cycle (if user context provided)
  /// </summary>
  public List<UserRequestHistoryDto> UserRequestHistory { get; init; } = new();
}