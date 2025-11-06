namespace Nezam.Refahi.Facilities.Application.Dtos;

/// <summary>
/// Enterprise-grade Facility data transfer object
/// Provides comprehensive facility information for client applications
/// </summary>
public record FacilityDto
{
  /// <summary>
  /// Unique facility identifier
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Facility display name
  /// </summary>
  public string Name { get; set; } = null!;

  /// <summary>
  /// Unique facility code for system identification
  /// </summary>
  public string Code { get; set; } = null!;

  /// <summary>
  /// Detailed facility description
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Associated bank information
  /// </summary>
  public BankInfoDto? BankInfo { get; set; }

  /// <summary>
  /// Current cycle statistics
  /// </summary>
  public FacilityCycleStatisticsDto? CycleStatistics { get; set; }

  /// <summary>
  /// Facility creation timestamp
  /// </summary>
  public DateTime CreatedAt { get; set; }

  /// <summary>
  /// Last modification timestamp
  /// </summary>
  public DateTime? LastModifiedAt { get; set; }

  /// <summary>
  /// Indicates if facility has any active cycles
  /// </summary>
  public bool HasActiveCycles => CycleStatistics?.ActiveCyclesCount > 0;

  /// <summary>
  /// Indicates if facility is accepting new applications
  /// </summary>
  public bool IsAcceptingApplications => HasActiveCycles;
}