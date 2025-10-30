namespace Nezam.Refahi.Recreation.Contracts.Dtos;

public sealed class CapacityDetailDto
{
  public Guid Id { get; set; } = Guid.Empty;
  public DateTime RegistrationStart { get; set; } = DateTime.MinValue;
  public DateTime RegistrationEnd { get; set; } = DateTime.MinValue;
  public int MaxParticipants { get; set; } = 0;
  public int RemainingParticipants { get; set; } = 0;
  public int AllocatedParticipants { get; set; } = 0;
  public int MinParticipantsPerReservation { get; set; } = 0;
  public int MaxParticipantsPerReservation { get; set; } = 0;
  public bool IsActive { get; set; } = false;
  public bool IsSpecial { get; set; } = false;
  public string CapacityState { get; set; } = string.Empty;
  public string? Description { get; set; } = string.Empty;
}