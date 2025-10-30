using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourPricingMapper : IMapper<TourPricing, PricingDetailDto>
{
    public Task<PricingDetailDto> MapAsync(TourPricing source, CancellationToken cancellationToken = default)
    {
        var effectivePrice = source.GetEffectivePrice();
        
        var dto = new PricingDetailDto
        {
            Id = source.Id,
            ParticipantType = source.ParticipantType.ToString(),
            BasePriceRials = source.BasePrice.AmountRials,
            EffectivePriceRials = effectivePrice.AmountRials,
            DiscountPercentage = source.DiscountPercentage,
            DiscountAmountRials = source.DiscountPercentage.HasValue
                ? (source.BasePrice.AmountRials - effectivePrice.AmountRials)
                : null,
            ValidFrom = source.ValidFrom,
            ValidTo = source.ValidTo,
            IsActive = source.IsActive,
            IsEarlyBird = source.IsEarlyBird,
            IsLastMinute = source.IsLastMinute,
            Description = source.Description
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourPricing source, PricingDetailDto destination, CancellationToken cancellationToken = default)
    {
        var effectivePrice = source.GetEffectivePrice();
        
        destination.Id = source.Id;
        destination.ParticipantType = source.ParticipantType.ToString();
        destination.BasePriceRials = source.BasePrice.AmountRials;
        destination.EffectivePriceRials = effectivePrice.AmountRials;
        destination.DiscountPercentage = source.DiscountPercentage;
        destination.DiscountAmountRials = source.DiscountPercentage.HasValue
            ? (source.BasePrice.AmountRials - effectivePrice.AmountRials)
            : null;
        destination.ValidFrom = source.ValidFrom;
        destination.ValidTo = source.ValidTo;
        destination.IsActive = source.IsActive;
        destination.IsEarlyBird = source.IsEarlyBird;
        destination.IsLastMinute = source.IsLastMinute;
        destination.Description = source.Description;

        return Task.CompletedTask;
    }
}

