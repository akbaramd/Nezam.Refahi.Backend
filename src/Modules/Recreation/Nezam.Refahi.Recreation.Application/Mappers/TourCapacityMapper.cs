using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class TourCapacityMapper : IMapper<TourCapacity, CapacityDetailDto>
{
    public Task<CapacityDetailDto> MapAsync(TourCapacity source, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var dto = new CapacityDetailDto
        {
            Id = source.Id,
            RegistrationStart = source.RegistrationStart,
            RegistrationEnd = source.RegistrationEnd,
            MaxParticipants = source.MaxParticipants,
            RemainingParticipants = source.RemainingParticipants,
            AllocatedParticipants = source.AllocatedParticipants,
            MinParticipantsPerReservation = source.MinParticipantsPerReservation,
            MaxParticipantsPerReservation = source.MaxParticipantsPerReservation,
            IsActive = source.IsActive,
            IsSpecial = source.IsSpecial,
            CapacityState = source.CapacityState.ToString(),
            IsRegistrationOpen = source.IsRegistrationOpen(now),
            IsFullyBooked = source.IsFullyBooked(),
            IsNearlyFull = source.IsNearlyFull(),
            Description = source.Description
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourCapacity source, CapacityDetailDto destination, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        destination.Id = source.Id;
        destination.RegistrationStart = source.RegistrationStart;
        destination.RegistrationEnd = source.RegistrationEnd;
        destination.MaxParticipants = source.MaxParticipants;
        destination.RemainingParticipants = source.RemainingParticipants;
        destination.AllocatedParticipants = source.AllocatedParticipants;
        destination.MinParticipantsPerReservation = source.MinParticipantsPerReservation;
        destination.MaxParticipantsPerReservation = source.MaxParticipantsPerReservation;
        destination.IsActive = source.IsActive;
        destination.IsSpecial = source.IsSpecial;
        destination.CapacityState = source.CapacityState.ToString();
        destination.IsRegistrationOpen = source.IsRegistrationOpen(now);
        destination.IsFullyBooked = source.IsFullyBooked();
        destination.IsNearlyFull = source.IsNearlyFull();
        destination.Description = source.Description;
        return Task.CompletedTask;
    }
}

