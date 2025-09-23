using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Contracts;

namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Detailed tour capacity data transfer object for reservations
/// </summary>
public class TourCapacityDetailDto : IStaticMapper<TourCapacity, TourCapacityDetailDto>
{
  public Guid Id { get; set; }
  public Guid TourId { get; set; }
  public int MaxParticipants { get; set; }
  public DateTime RegistrationStart { get; set; }
  public DateTime RegistrationEnd { get; set; }
  public bool IsActive { get; set; }
  public string? Description { get; set; }
    
  // Enhanced capacity properties
  public string? Name { get; set; }
  public int RemainingParticipants { get; set; }
  public int AllocatedParticipants { get; set; }
  public int MinParticipantsPerReservation { get; set; }
  public int MaxParticipantsPerReservation { get; set; }
  public bool IsFullyBooked { get; set; }
  public bool IsNearlyFull { get; set; }
  public CapacityState CapacityState { get; set; }
  public string AvailabilityStatus { get; set; } = string.Empty;
  public string AvailabilityMessage { get; set; } = string.Empty;
  public string? Color { get; set; }
  public string? Icon { get; set; }
  public string? VehicleType { get; set; }
  public string? VehicleNumber { get; set; }
  public string? DriverInfo { get; set; }
  public string? Notes { get; set; }
  public int DisplayOrder { get; set; }
    
  // Calculated properties
  public bool IsRegistrationOpen { get; set; }
  public bool IsEffectiveFor { get; set; }
  public int CurrentUtilization { get; set; }
  public int AvailableSpots { get; set; }
  public double UtilizationPercentage { get; set; }
  public bool IsAtCapacity { get; set; }
    
  // Time-related properties
  public TimeSpan? TimeUntilRegistrationStart { get; set; }
  public TimeSpan? TimeUntilRegistrationEnd { get; set; }
  public bool IsExpired { get; set; }

  /// <summary>
  /// Maps from TourCapacity entity to TourCapacityDetailDto
  /// Note: This is a basic mapping without real-time utilization data
  /// </summary>
  /// <param name="entity">The source capacity entity</param>
  /// <returns>Mapped TourCapacityDetailDto</returns>
  public static TourCapacityDetailDto MapFrom(TourCapacity entity)
  {
    var currentDate = DateTime.UtcNow;
    var timeUntilStart = entity.RegistrationStart > currentDate 
      ? entity.RegistrationStart - currentDate 
      : (TimeSpan?)null;
    var timeUntilEnd = entity.RegistrationEnd > currentDate 
      ? entity.RegistrationEnd - currentDate 
      : (TimeSpan?)null;

    return new TourCapacityDetailDto
    {
      Id = entity.Id,
      TourId = entity.TourId,
      MaxParticipants = entity.MaxParticipants,
      RegistrationStart = entity.RegistrationStart,
      RegistrationEnd = entity.RegistrationEnd,
      IsActive = entity.IsActive,
      Description = entity.Description,
      IsRegistrationOpen = entity.IsRegistrationOpen(currentDate),
      IsEffectiveFor = entity.IsEffectiveFor(currentDate),
      CurrentUtilization = 0, // Basic mapping - use query handler for real data
      AvailableSpots = entity.MaxParticipants, // Basic mapping - use query handler for real data
      UtilizationPercentage = 0, // Basic mapping - use query handler for real data
      IsAtCapacity = false, // Basic mapping - use query handler for real data
      TimeUntilRegistrationStart = timeUntilStart,
      TimeUntilRegistrationEnd = timeUntilEnd,
      IsExpired = entity.RegistrationEnd < currentDate
    };
  }

  /// <summary>
  /// Maps from TourCapacityDetailDto to TourCapacity entity
  /// Note: This operation is not supported as DTOs should not create entities
  /// </summary>
  /// <param name="dto">The DTO to map from</param>
  /// <returns>Not supported</returns>
  public static TourCapacity MapTo(TourCapacityDetailDto dto)
  {
    throw new NotSupportedException("Mapping from TourCapacityDetailDto to TourCapacity is not supported. Use domain services to create entities.");
  }
}