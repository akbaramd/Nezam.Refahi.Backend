using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class ReservationSummaryMapper : IMapper<TourReservation, ReservationSummaryDto>
{
    public Task<ReservationSummaryDto> MapAsync(TourReservation source, CancellationToken cancellationToken = default)
    {
        var dto = new ReservationSummaryDto
        {
            Id = source.Id,
            TourId = source.TourId,
            TrackingCode = source.TrackingCode,
            Status = source.Status.ToString(),
            ReservationDate = source.ReservationDate
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourReservation source, ReservationSummaryDto destination, CancellationToken cancellationToken = default)
    {
        destination.Id = source.Id;
        destination.TourId = source.TourId;
        destination.TrackingCode = source.TrackingCode;
        destination.Status = source.Status.ToString();
        destination.ReservationDate = source.ReservationDate;
        return Task.CompletedTask;
    }
}


