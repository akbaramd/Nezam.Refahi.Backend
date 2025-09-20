using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Contracts;

namespace Nezam.Refahi.Recreation.Contracts.Dtos;

/// <summary>
/// Tour pricing data transfer object
/// </summary>
public class TourPricingDto : IStaticMapper<TourPricing, TourPricingDto>
{
  public Guid Id { get; set; }
  public ParticipantType ParticipantType { get; set; }
  public decimal BasePrice { get; set; }
  public decimal? DiscountPercentage { get; set; }
  public decimal EffectivePrice { get; set; }
  public DateTime? ValidFrom { get; set; }
  public DateTime? ValidTo { get; set; }
  public bool IsActive { get; set; }

  /// <summary>
  /// Maps from TourPricing entity to TourPricingDto
  /// </summary>
  /// <param name="entity">The source pricing entity</param>
  /// <returns>Mapped TourPricingDto</returns>
  public static TourPricingDto MapFrom(TourPricing entity)
  {
    var effectivePrice = entity.GetEffectivePrice();
        
    return new TourPricingDto
    {
      Id = entity.Id,
      ParticipantType = entity.ParticipantType,
      BasePrice = entity.Price.AmountRials,
      DiscountPercentage = entity.DiscountPercentage,
      EffectivePrice = effectivePrice.AmountRials,
      ValidFrom = entity.ValidFrom,
      ValidTo = entity.ValidTo,
      IsActive = entity.IsActive
    };
  }

  /// <summary>
  /// Maps from TourPricingDto to TourPricing entity
  /// Note: This operation is not supported as DTOs should not create entities
  /// </summary>
  /// <param name="dto">The DTO to map from</param>
  /// <returns>Not supported</returns>
  public static TourPricing MapTo(TourPricingDto dto)
  {
    throw new NotSupportedException("Mapping from TourPricingDto to TourPricing is not supported. Use domain services to create entities.");
  }
}