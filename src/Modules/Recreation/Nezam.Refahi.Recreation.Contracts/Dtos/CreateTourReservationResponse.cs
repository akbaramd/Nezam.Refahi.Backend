namespace Nezam.Refahi.Recreation.Application.Dtos;

/// <summary>
/// Response after creating tour reservation
/// </summary>
public class CreateTourReservationResponse
{
  public Guid ReservationId { get; set; } = Guid.Empty;
  public string TrackingCode { get; set; } = string.Empty;
  public DateTime ReservationDate { get; set; } = DateTime.MinValue;
  public DateTime? ExpiryDate { get; set; } = null;
  public string TourTitle { get; set; } = string.Empty;
  public int TotalParticipants { get; set; } = 0;
  public decimal? EstimatedTotalPrice { get; set; } = null;
}