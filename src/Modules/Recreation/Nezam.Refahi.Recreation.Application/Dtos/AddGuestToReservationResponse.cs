namespace Nezam.Refahi.Recreation.Application.Dtos;

/// <summary>
/// Response after adding guest to reservation
/// </summary>
public class AddGuestToReservationResponse
{
  public Guid ParticipantId { get; init; }
  public string TrackingCode { get; init; } = null!;
  public int TotalParticipants { get; init; }
  public decimal? UpdatedTotalPrice { get; init; }
  public string GuestName { get; init; } = null!;
}