namespace Nezam.Refahi.Recreation.Application.Dtos;

/// <summary>
/// Pricing information for a reservation
/// </summary>
public class ReservationPricingResponse
{
  public Guid ReservationId { get; init; }
  public string TrackingCode { get; init; } = null!;
  public string TourTitle { get; init; } = null!;
    
  // Tour information
  public DateTime? TourStart { get; init; }
  public DateTime? TourEnd { get; init; }
    
  // Reservation details
  public DateTime ReservationDate { get; init; }
  public DateTime? ExpiryDate { get; init; }
  public DateTime? ConfirmationDate { get; init; }
    
  // Pricing details
  public List<ParticipantPricingDto> ParticipantPricing { get; init; } = [];
  public decimal TotalRequiredAmount { get; init; }
  public decimal TotalPaidAmount { get; init; }
  public decimal TotalRemainingAmount { get; init; }
  public bool IsFullyPaid { get; init; }
  public DateTime? PaymentDeadline { get; init; }
    
  // Status information
  public string Status { get; init; } = null!;
  public bool IsExpired { get; init; }
  public bool IsPending { get; init; }
  public bool IsConfirmed { get; init; }
    
  // Participant summary
  public int ParticipantCount { get; init; }
  public int MainParticipantCount { get; init; }
  public int GuestParticipantCount { get; init; }
}