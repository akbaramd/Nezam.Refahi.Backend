namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Lightweight reservation summary for embedding in other DTOs and list items.
/// Only contains the most important identifiers and status/date fields.
/// </summary>
public class ReservationSummaryDto
{
  public Guid Id { get; set; } = Guid.Empty;
  public Guid TourId { get; set; } = Guid.Empty;
  public string TrackingCode { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public DateTime ReservationDate { get; set; } = DateTime.MinValue;
}


