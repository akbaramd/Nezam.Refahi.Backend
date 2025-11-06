using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourWithUserReservationMapper : IMapper<Tour, TourWithUserReservationDto>
{
    private readonly IMapper<Tour, TourDto> _tourMapper;

    public TourWithUserReservationMapper(IMapper<Tour, TourDto> tourMapper)
    {
        _tourMapper = tourMapper ?? throw new ArgumentNullException(nameof(tourMapper));
    }

    public async Task<TourWithUserReservationDto> MapAsync(Tour source, CancellationToken cancellationToken = default)
    {
        var baseDto = await _tourMapper.MapAsync(source, cancellationToken);

        var dto = new TourWithUserReservationDto
        {
            Id = baseDto.Id,
            Title = baseDto.Title,
            TourStart = baseDto.TourStart,
            TourEnd = baseDto.TourEnd,
            IsActive = baseDto.IsActive,
            Status = baseDto.Status,
            CapacityState = baseDto.CapacityState,
            RegistrationStart = baseDto.RegistrationStart,
            RegistrationEnd = baseDto.RegistrationEnd,
            IsRegistrationOpen = baseDto.IsRegistrationOpen,
            MaxCapacity = baseDto.MaxCapacity,
            RemainingCapacity = baseDto.RemainingCapacity,
            ReservedCapacity = baseDto.ReservedCapacity,
            UtilizationPct = baseDto.UtilizationPct,
            IsFullyBooked = baseDto.IsFullyBooked,
            IsNearlyFull = baseDto.IsNearlyFull,
            Agencies = baseDto.Agencies,
            Features = baseDto.Features,
            Photos = baseDto.Photos,
            LowestPriceRials = baseDto.LowestPriceRials,
            HighestPriceRials = baseDto.HighestPriceRials,
            HasDiscount = baseDto.HasDiscount,
            Pricing = baseDto.Pricing,
            // Reservation is set by the query layer using ReservationSummaryMapper
            Reservation = null
        };

        return dto;
    }

    public async Task MapAsync(Tour source, TourWithUserReservationDto destination, CancellationToken cancellationToken = default)
    {
        var baseDto = await _tourMapper.MapAsync(source, cancellationToken);

        destination.Id = baseDto.Id;
        destination.Title = baseDto.Title;
        destination.TourStart = baseDto.TourStart;
        destination.TourEnd = baseDto.TourEnd;
        destination.IsActive = baseDto.IsActive;
        destination.Status = baseDto.Status;
        destination.CapacityState = baseDto.CapacityState;
        destination.RegistrationStart = baseDto.RegistrationStart;
        destination.RegistrationEnd = baseDto.RegistrationEnd;
        destination.IsRegistrationOpen = baseDto.IsRegistrationOpen;
        destination.MaxCapacity = baseDto.MaxCapacity;
        destination.RemainingCapacity = baseDto.RemainingCapacity;
        destination.ReservedCapacity = baseDto.ReservedCapacity;
        destination.UtilizationPct = baseDto.UtilizationPct;
        destination.IsFullyBooked = baseDto.IsFullyBooked;
        destination.IsNearlyFull = baseDto.IsNearlyFull;
        destination.Agencies = baseDto.Agencies;
        destination.Features = baseDto.Features;
        destination.Photos = baseDto.Photos;
        destination.LowestPriceRials = baseDto.LowestPriceRials;
        destination.HighestPriceRials = baseDto.HighestPriceRials;
        destination.HasDiscount = baseDto.HasDiscount;
        destination.Pricing = baseDto.Pricing;
        // Destination.Reservation is intentionally not set here
    }
}


