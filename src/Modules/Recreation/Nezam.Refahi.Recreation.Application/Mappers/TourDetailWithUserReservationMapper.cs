using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourDetailWithUserReservationMapper : IMapper<Tour, TourDetailWithUserReservationDto>
{
    private readonly IMapper<Tour, TourDetailDto> _tourDetailMapper;

    public TourDetailWithUserReservationMapper(IMapper<Tour, TourDetailDto> tourDetailMapper)
    {
        _tourDetailMapper = tourDetailMapper ?? throw new ArgumentNullException(nameof(tourDetailMapper));
    }

    public async Task<TourDetailWithUserReservationDto> MapAsync(Tour source, CancellationToken cancellationToken = default)
    {
        var baseDto = await _tourDetailMapper.MapAsync(source, cancellationToken);

        var dto = new TourDetailWithUserReservationDto
        {
            // Base TourDto fields
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

            // TourDetailDto fields
            Description = baseDto.Description,
            LongDescription = baseDto.LongDescription,
            Summary = baseDto.Summary,
            MinAge = baseDto.MinAge,
            MaxAge = baseDto.MaxAge,
            MaxGuestsPerReservation = baseDto.MaxGuestsPerReservation,
            RequiredCapabilities = baseDto.RequiredCapabilities,
            RequiredFeatures = baseDto.RequiredFeatures,
            Capacities = baseDto.Capacities,
            RestrictedTours = baseDto.RestrictedTours,
            TotalReservations = baseDto.TotalReservations,
            ConfirmedReservations = baseDto.ConfirmedReservations,
            PendingReservations = baseDto.PendingReservations,
            CancelledReservations = baseDto.CancelledReservations,
            UserReservationTrackingCode = baseDto.UserReservationTrackingCode,
            UserReservationDate = baseDto.UserReservationDate,
            UserReservationExpiryDate = baseDto.UserReservationExpiryDate,

            // Reservation will be set by query layer using ReservationMapper
            Reservation = null
        };

        // Features/Photos/Agencies already set from baseDto
        return dto;
    }

    public async Task MapAsync(Tour source, TourDetailWithUserReservationDto destination, CancellationToken cancellationToken = default)
    {
        var baseDto = await _tourDetailMapper.MapAsync(source, cancellationToken);

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

        destination.Description = baseDto.Description;
        destination.LongDescription = baseDto.LongDescription;
        destination.Summary = baseDto.Summary;
        destination.MinAge = baseDto.MinAge;
        destination.MaxAge = baseDto.MaxAge;
        destination.MaxGuestsPerReservation = baseDto.MaxGuestsPerReservation;
        destination.RequiredCapabilities = baseDto.RequiredCapabilities;
        destination.RequiredFeatures = baseDto.RequiredFeatures;
        destination.Capacities = baseDto.Capacities;
        destination.RestrictedTours = baseDto.RestrictedTours;
        destination.TotalReservations = baseDto.TotalReservations;
        destination.ConfirmedReservations = baseDto.ConfirmedReservations;
        destination.PendingReservations = baseDto.PendingReservations;
        destination.CancelledReservations = baseDto.CancelledReservations;
        destination.UserReservationTrackingCode = baseDto.UserReservationTrackingCode;
        destination.UserReservationDate = baseDto.UserReservationDate;
        destination.UserReservationExpiryDate = baseDto.UserReservationExpiryDate;
        // Destination.Reservation is intentionally not set here
    }
}


