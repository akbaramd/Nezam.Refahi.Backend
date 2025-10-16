using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;

/// <summary>
/// Enterprise-grade Facility data transfer object
/// Provides comprehensive facility information for client applications
/// </summary>
public record FacilityDto
{
  /// <summary>
  /// Unique facility identifier
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Facility display name
  /// </summary>
  public string Name { get; init; } = null!;

  /// <summary>
  /// Unique facility code for system identification
  /// </summary>
  public string Code { get; init; } = null!;

  /// <summary>
  /// Facility type (Loan, Grant, Card, WelfareVoucher, Other)
  /// </summary>
  public string Type { get; init; } = null!;

  /// <summary>
  /// Human-readable facility type text
  /// </summary>
  public string TypeText { get; init; } = null!;

  /// <summary>
  /// Current facility status (Draft, Active, Suspended, Closed, Maintenance)
  /// </summary>
  public string Status { get; init; } = null!;

  /// <summary>
  /// Human-readable status text
  /// </summary>
  public string StatusText { get; init; } = null!;

  /// <summary>
  /// Indicates if facility is currently active and accepting applications
  /// </summary>
  public bool IsActive { get; init; }

  /// <summary>
  /// Detailed facility description
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Associated bank information
  /// </summary>
  public BankInfoDto? BankInfo { get; init; }

  /// <summary>
  /// Current cycle statistics
  /// </summary>
  public FacilityCycleStatisticsDto? CycleStatistics { get; init; }

  /// <summary>
  /// Facility metadata and additional properties
  /// </summary>
  public Dictionary<string, string> Metadata { get; init; } = new();

  /// <summary>
  /// Facility creation timestamp
  /// </summary>
  public DateTime CreatedAt { get; init; }

  /// <summary>
  /// Last modification timestamp
  /// </summary>
  public DateTime? LastModifiedAt { get; init; }

  /// <summary>
  /// Indicates if facility has any active cycles
  /// </summary>
  public bool HasActiveCycles => CycleStatistics?.ActiveCyclesCount > 0;

  /// <summary>
  /// Indicates if facility is accepting new applications
  /// </summary>
  public bool IsAcceptingApplications => IsActive && HasActiveCycles;
}