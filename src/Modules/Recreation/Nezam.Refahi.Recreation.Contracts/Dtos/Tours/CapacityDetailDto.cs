namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Detailed capacity information for a tour, including availability status fields.
/// All status fields are calculated using domain behaviors for consistency.
/// </summary>
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
  
  /// <summary>
  /// Indicates whether registration is currently open for this capacity.
  /// True when: IsActive && currentDate >= RegistrationStart && currentDate <= RegistrationEnd
  /// </summary>
  public bool IsRegistrationOpen { get; set; } = false;
  
  /// <summary>
  /// Indicates whether this capacity is completely full (RemainingParticipants <= 0).
  /// Use this to prevent booking attempts when no spots are available.
  /// </summary>
  public bool IsFullyBooked { get; set; } = false;
  
  /// <summary>
  /// Indicates whether this capacity is nearly full (≥80% utilized but not fully booked).
  /// Use this to show urgency indicators to encourage faster booking.
  /// </summary>
  public bool IsNearlyFull { get; set; } = false; // ≥80% utilized
  
  public string? Description { get; set; } = string.Empty;
}