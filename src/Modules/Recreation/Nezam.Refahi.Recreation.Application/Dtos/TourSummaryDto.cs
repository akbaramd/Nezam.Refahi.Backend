using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Shared.Application.Common.Contracts;

namespace Nezam.Refahi.Recreation.Application.Dtos;

/// <summary>
/// Tour summary data transfer object for reservation details
/// </summary>
public class TourSummaryDto : IStaticMapper<Tour, TourSummaryDto>
{
  public Guid Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public DateTime TourStart { get; set; }
  public DateTime TourEnd { get; set; }

  // Capacity information
  public int MaxCapacity { get; set; }
  public bool IsAtCapacity { get; set; }

  // Registration period (calculated from active capacities)
  public DateTime? RegistrationStart { get; set; }
  public DateTime? RegistrationEnd { get; set; }
  public bool IsRegistrationOpen { get; set; }

  // Age restrictions
  public int? MinAge { get; set; }
  public int? MaxAge { get; set; }

  // Guest limitations per reservation
  public int? MaxGuestsPerReservation { get; set; }

  public bool IsActive { get; set; }

  // Capacity details
  public List<TourCapacityDetailDto> Capacities { get; set; } = new();
    
  // Current reservation statistics
  public int CurrentReservations { get; set; }
  public int AvailableSpots { get; set; }

    
  // Pricing information
  public List<TourPricingDto> Pricing { get; set; } = new();

  /// <summary>
  /// Maps from Tour entity to TourSummaryDto
  /// Note: This is a basic mapping. Use the query handler for complete mapping with context
  /// </summary>
  /// <param name="entity">The source tour entity</param>
  /// <returns>Mapped TourSummaryDto</returns>
  public static TourSummaryDto MapFrom(Tour entity)
  {
    var currentDate = DateTime.UtcNow;
    var activeCapacities = entity.Capacities.Where(c => c.IsActive).ToList();
        
    return new TourSummaryDto
    {
      Id = entity.Id,
      Title = entity.Title,
      Description = entity.Description,
      TourStart = entity.TourStart,
      TourEnd = entity.TourEnd,
      MaxCapacity = activeCapacities.Sum(c => c.MaxParticipants),
      CurrentReservations = entity.GetConfirmedReservationCount() + entity.GetPendingReservationCount(),
      AvailableSpots = entity.GetAvailableSpots(),
      IsAtCapacity = !entity.HasAvailableSpotsForReservation(),
      RegistrationStart = activeCapacities.Any() ? activeCapacities.Min(c => c.RegistrationStart) : null,
      RegistrationEnd = activeCapacities.Any() ? activeCapacities.Max(c => c.RegistrationEnd) : null,
      IsRegistrationOpen = entity.IsRegistrationOpen(currentDate),
      MinAge = entity.MinAge,
      MaxAge = entity.MaxAge,
      MaxGuestsPerReservation = entity.MaxGuestsPerReservation,
      IsActive = entity.IsActive,
      Capacities = activeCapacities.Select(TourCapacityDetailDto.MapFrom).ToList(),
      Pricing = entity.Pricing.Select(p => TourPricingDto.MapFrom(p)).ToList()
    };
  }

  /// <summary>
  /// Maps from TourSummaryDto to Tour entity
  /// Note: This operation is not supported as DTOs should not create entities
  /// </summary>
  /// <param name="dto">The DTO to map from</param>
  /// <returns>Not supported</returns>
  public static Tour MapTo(TourSummaryDto dto)
  {
    throw new NotSupportedException("Mapping from TourSummaryDto to Tour is not supported. Use domain services to create entities.");
  }
}