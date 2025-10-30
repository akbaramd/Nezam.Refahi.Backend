using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class CapacitySummaryMapper : IMapper<TourCapacity, CapacitySummaryDto>
{
    public Task<CapacitySummaryDto> MapAsync(TourCapacity source, CancellationToken cancellationToken = default)
    {
        var dto = new CapacitySummaryDto
        {
            Id = source.Id,
            TourId = source.TourId,
            MaxParticipants = source.MaxParticipants,
            RegistrationStart = source.RegistrationStart,
            RegistrationEnd = source.RegistrationEnd,
            IsActive = source.IsActive,
            CapacityState = source.CapacityState.ToString(),
            Description = source.Description
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourCapacity source, CapacitySummaryDto destination, CancellationToken cancellationToken = default)
    {
        destination.Id = source.Id;
        destination.TourId = source.TourId;
        destination.MaxParticipants = source.MaxParticipants;
        destination.RegistrationStart = source.RegistrationStart;
        destination.RegistrationEnd = source.RegistrationEnd;
        destination.IsActive = source.IsActive;
        destination.CapacityState = source.CapacityState.ToString();
        destination.Description = source.Description;
        return Task.CompletedTask;
    }
}

