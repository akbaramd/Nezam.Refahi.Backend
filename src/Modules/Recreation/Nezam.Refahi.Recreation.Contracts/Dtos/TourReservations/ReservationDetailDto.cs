namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Compact reservation row for list screens. No heavy relations.
/// </summary>
public class ReservationDto
{
  // Identity
  public Guid Id { get; set; } = Guid.Empty;
  public Guid TourId { get; set; } = Guid.Empty;
  public string TrackingCode { get; set; } = string.Empty;

  // Status + dates
  public string Status { get; set; } = string.Empty;
  public DateTime ReservationDate { get; set; } = DateTime.MinValue;
  public DateTime? ExpiryDate { get; set; } = null;
  public DateTime? ConfirmationDate { get; set; } = null;

  // Amounts (nullable to reflect missing totals)
  public decimal? TotalAmountRials { get; set; } = null;
  public decimal? PaidAmountRials { get; set; } = null;
  public decimal? RemainingAmountRials { get; set; } = null;
  public bool IsFullyPaid { get; set; } = false;

  // Counts
  public int ParticipantCount { get; set; } = 0;
  public int MainParticipantCount { get; set; } = 0;
  public int GuestParticipantCount { get; set; } = 0;

  // Quick flags (derived from entity methods)
  public bool IsExpired { get; set; } = false;
  public bool IsConfirmed { get; set; } = false;
  public bool IsPending { get; set; } = false;
  public bool IsDraft { get; set; } = false;
  public bool IsPaying { get; set; } = false;
  public bool IsCancelled { get; set; } = false;
  public bool IsTerminal { get; set; } = false;

  // Lightweight cross-links
  public Guid? CapacityId { get; set; } = null;
  public Guid? BillId { get; set; } = null;

  // Optional tour hints (null-safe; only from loaded navigation)
  public string? TourTitle { get; set; } = string.Empty;
  public DateTime? TourStart { get; set; } = null;
  public DateTime? TourEnd { get; set; } = null;
  public string? TourStatus { get; set; } = string.Empty;
  public bool? TourIsActive { get; set; } = null;
}

/// <summary>
/// Detailed reservation view: includes participants, capacity summary, price snapshots.
/// Uses only data present on the aggregate and its navigations.
/// </summary>
public sealed class ReservationDetailDto : ReservationDto
{
  public DateTime? CancellationDate { get; set; } = null;
  public string? CancellationReason { get; set; } = string.Empty;

  public Guid? MemberId { get; set; } = null;
  public Guid ExternalUserId { get; set; } = Guid.Empty;

  // Capacity (safe summary)
  public CapacitySummaryDto? Capacity { get; set; } = null;

  // Tour (safe summary)
  public TourBriefDto? Tour { get; set; } = null;

  // Participants and pricing snapshots
  public List<ParticipantDto> Participants { get; set; } = new();
  public List<PriceSnapshotDto> PriceSnapshots { get; set; } = new();

  // Notes / misc
  public string? Notes { get; set; } = string.Empty;
  public string? TenantId { get; set; } = string.Empty;

  // Audit
  public DateTime CreatedAt { get; set; } = DateTime.MinValue;
  public DateTime? UpdatedAt { get; set; } = null;
  public string CreatedBy { get; set; } = string.Empty;
  public string? UpdatedBy { get; set; } = string.Empty;
}

/// <summary>
/// Minimal capacity info used inside reservation details. Maps strictly from TourCapacity.
/// </summary>
public sealed class CapacitySummaryDto
{
  public Guid Id { get; set; } = Guid.Empty;
  public Guid TourId { get; set; } = Guid.Empty;    
  public int MaxParticipants { get; set; } = 0;
  public DateTime RegistrationStart { get; set; } = DateTime.MinValue;
  public DateTime RegistrationEnd { get; set; } = DateTime.MinValue;
  public bool IsActive { get; set; } = false;
  public string CapacityState { get; set; } = string.Empty;
  public string? Description { get; set; } = string.Empty;
}

/// <summary>
/// Minimal tour info embedded in reservation details. Maps strictly from Tour.
/// </summary>
public sealed class TourBriefDto
{
  public Guid Id { get; set; } = Guid.Empty;
  public string Title { get; set; } = string.Empty;
  public DateTime TourStart { get; set; } = DateTime.MinValue;
  public DateTime TourEnd { get; set; } = DateTime.MinValue;
  public string Status { get; set; } = string.Empty;
  public bool IsActive { get; set; } = false;
}

/// <summary>
/// Minimal snapshot of a per-type price captured on reservation. Uses only fields referenced by the entity.
/// </summary>
public sealed class PriceSnapshotDto
{
  public string ParticipantType { get; set; } = string.Empty;
  public decimal FinalPriceRials { get; set; } = 0;
}
