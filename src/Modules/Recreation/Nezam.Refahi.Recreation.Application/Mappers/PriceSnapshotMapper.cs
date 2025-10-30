using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class PriceSnapshotMapper : IMapper<ReservationPriceSnapshot, PriceSnapshotDto>
{
    public Task<PriceSnapshotDto> MapAsync(ReservationPriceSnapshot source, CancellationToken cancellationToken = default)
    {
        var dto = new PriceSnapshotDto
        {
            ParticipantType = source.ParticipantType.ToString(),
            FinalPriceRials = source.FinalPrice.AmountRials
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(ReservationPriceSnapshot source, PriceSnapshotDto destination, CancellationToken cancellationToken = default)
    {
        // DTOs use init-only properties, so update mapping is not supported
        throw new NotImplementedException("Update mapping not supported for DTOs with init-only properties. Use MapAsync(ReservationPriceSnapshot) instead.");
    }
}

