namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Response after adding guest to reservation
/// </summary>
public class AddGuestToReservationResponse
{
  public Guid ParticipantId { get; set; } = Guid.Empty;
  public Guid ReservationId { get; set; } = Guid.Empty;
  public Guid? CapacityId { get; set; } = null;
}