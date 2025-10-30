using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class ReservationDetailMapper : IMapper<TourReservation, ReservationDetailDto>
{
    private readonly IMapper<TourReservation, ReservationDto> _reservationMapper;
    private readonly IMapper<TourCapacity, CapacitySummaryDto> _capacityMapper;
    private readonly IMapper<Tour, TourBriefDto> _tourMapper;
    private readonly IMapper<Participant, ParticipantDto> _participantMapper;
    private readonly IMapper<ReservationPriceSnapshot, PriceSnapshotDto> _priceSnapshotMapper;

    public ReservationDetailMapper(
        IMapper<TourReservation, ReservationDto> reservationMapper,
        IMapper<TourCapacity, CapacitySummaryDto> capacityMapper,
        IMapper<Tour, TourBriefDto> tourMapper,
        IMapper<Participant, ParticipantDto> participantMapper,
        IMapper<ReservationPriceSnapshot, PriceSnapshotDto> priceSnapshotMapper)
    {
        _reservationMapper = reservationMapper;
        _capacityMapper = capacityMapper;
        _tourMapper = tourMapper;
        _participantMapper = participantMapper;
        _priceSnapshotMapper = priceSnapshotMapper;
    }

    public async Task<ReservationDetailDto> MapAsync(TourReservation source, CancellationToken cancellationToken = default)
    {
        // Start from base ReservationDto
        var baseDto = await _reservationMapper.MapAsync(source, cancellationToken);

        var participants = source.Participants ?? Array.Empty<Participant>();
        var mainCount = participants.Count(p => p.ParticipantType == ParticipantType.Member);
        var guestCount = participants.Count(p => p.ParticipantType == ParticipantType.Guest);

        var remaining = source.RemainingAmount.AmountRials;

        // Map related entities
        CapacitySummaryDto? capacityDto = null;
        if (source.Capacity != null)
            capacityDto = await _capacityMapper.MapAsync(source.Capacity, cancellationToken);

        TourBriefDto? tourDto = null;
        if (source.Tour != null)
            tourDto = await _tourMapper.MapAsync(source.Tour, cancellationToken);

        var participantDtos = await Task.WhenAll(
            participants.Select(p => _participantMapper.MapAsync(p, cancellationToken)));

        var snapshotDtos = await Task.WhenAll(
            (source.PriceSnapshots ?? Array.Empty<ReservationPriceSnapshot>())
                .Select(ps => _priceSnapshotMapper.MapAsync(ps, cancellationToken)));

        return new ReservationDetailDto
        {
            // Base properties
            Id = baseDto.Id,
            TourId = baseDto.TourId,
            TrackingCode = baseDto.TrackingCode,
            Status = baseDto.Status,
            ReservationDate = baseDto.ReservationDate,
            ExpiryDate = baseDto.ExpiryDate,
            ConfirmationDate = baseDto.ConfirmationDate,
            TotalAmountRials = baseDto.TotalAmountRials,
            PaidAmountRials = baseDto.PaidAmountRials,
            RemainingAmountRials = remaining,
            IsFullyPaid = baseDto.IsFullyPaid,
            ParticipantCount = participants.Count,
            MainParticipantCount = mainCount,
            GuestParticipantCount = guestCount,
            IsExpired = baseDto.IsExpired,
            IsConfirmed = baseDto.IsConfirmed,
            IsPending = baseDto.IsPending,
            IsDraft = baseDto.IsDraft,
            IsPaying = baseDto.IsPaying,
            IsCancelled = baseDto.IsCancelled,
            IsTerminal = baseDto.IsTerminal,
            CapacityId = baseDto.CapacityId,
            BillId = baseDto.BillId,
            TourTitle = baseDto.TourTitle,
            TourStart = baseDto.TourStart,
            TourEnd = baseDto.TourEnd,
            TourStatus = baseDto.TourStatus,
            TourIsActive = baseDto.TourIsActive,

            // Detail-only properties
            CancellationDate = source.CancellationDate,
            CancellationReason = source.CancellationReason,
            MemberId = source.MemberId,
            ExternalUserId = source.ExternalUserId,
            Capacity = capacityDto,
            Tour = tourDto,
            Participants = participantDtos.ToList(),
            PriceSnapshots = snapshotDtos.ToList(),
            Notes = source.Notes,
            TenantId = source.TenantId,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.LastModifiedAt,
            CreatedBy = source.CreatedBy,
            UpdatedBy = source.LastModifiedBy
        };
    }

    public Task MapAsync(TourReservation source, ReservationDetailDto destination, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Update mapping not implemented. Use MapAsync(TourReservation) instead.");
    }
}

