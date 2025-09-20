namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Response after creating tour reservation
/// </summary>
public class CreateTourReservationResponse
{
  public Guid ReservationId { get; init; }
  public string TrackingCode { get; init; } = null!;
  public DateTime ReservationDate { get; init; }
  public DateTime? ExpiryDate { get; init; }
  public string TourTitle { get; init; } = null!;
  public int TotalParticipants { get; init; }
  public decimal? EstimatedTotalPrice { get; init; }
}