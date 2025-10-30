using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Application.Mappers;

public class ReservationMapper : IMapper<TourReservation, ReservationDto>
{
    public Task<ReservationDto> MapAsync(TourReservation source, CancellationToken cancellationToken = default)
    {
        var total = source.TotalAmount?.AmountRials;
        var paid = source.PaidAmount?.AmountRials;
        var remaining = source.TotalAmount is null
            ? (decimal?)null
            : source.RemainingAmount.AmountRials;

        var participants = source.Participants ?? Array.Empty<Participant>();
        var mainCount = participants.Count(p => p.ParticipantType == ParticipantType.Member);
        var guestCount = participants.Count(p => p.ParticipantType == ParticipantType.Guest);

        var dto = new ReservationDto
        {
            Id = source.Id,
            TourId = source.TourId,
            TrackingCode = source.TrackingCode,
            Status = source.Status.ToString(),
            ReservationDate = source.ReservationDate,
            ExpiryDate = source.ExpiryDate,
            ConfirmationDate = source.ConfirmationDate,
            TotalAmountRials = total,
            PaidAmountRials = paid,
            RemainingAmountRials = remaining,
            IsFullyPaid = source.IsFullyPaid,
            ParticipantCount = participants.Count,
            MainParticipantCount = mainCount,
            GuestParticipantCount = guestCount,
            IsExpired = source.IsExpired(),
            IsConfirmed = source.IsConfirmed(),
            IsPending = source.IsPending(),
            IsDraft = source.IsDraft(),
            IsPaying = source.IsPaying(),
            IsCancelled = source.IsCancelled(),
            IsTerminal = source.IsTerminal(),
            CapacityId = source.CapacityId,
            BillId = source.BillId,
            TourTitle = source.Tour?.Title,
            TourStart = source.Tour?.TourStart,
            TourEnd = source.Tour?.TourEnd,
            TourStatus = source.Tour?.Status.ToString(),
            TourIsActive = source.Tour?.IsActive
        };

        return Task.FromResult(dto);
    }

    public Task MapAsync(TourReservation source, ReservationDto destination, CancellationToken cancellationToken = default)
    {
        var total = source.TotalAmount?.AmountRials;
        var paid = source.PaidAmount?.AmountRials;
        var remaining = source.TotalAmount is null
            ? (decimal?)null
            : source.RemainingAmount.AmountRials;

        var participants = source.Participants ?? Array.Empty<Participant>();
        var mainCount = participants.Count(p => p.ParticipantType == ParticipantType.Member);
        var guestCount = participants.Count(p => p.ParticipantType == ParticipantType.Guest);

        destination.Id = source.Id;
        destination.TourId = source.TourId;
        destination.TrackingCode = source.TrackingCode;
        destination.Status = source.Status.ToString();
        destination.ReservationDate = source.ReservationDate;
        destination.ExpiryDate = source.ExpiryDate;
        destination.ConfirmationDate = source.ConfirmationDate;
        destination.TotalAmountRials = total;
        destination.PaidAmountRials = paid;
        destination.RemainingAmountRials = remaining;
        destination.IsFullyPaid = source.IsFullyPaid;
        destination.ParticipantCount = participants.Count;
        destination.MainParticipantCount = mainCount;
        destination.GuestParticipantCount = guestCount;
        destination.IsExpired = source.IsExpired();
        destination.IsConfirmed = source.IsConfirmed();
        destination.IsPending = source.IsPending();
        destination.IsDraft = source.IsDraft();
        destination.IsPaying = source.IsPaying();
        destination.IsCancelled = source.IsCancelled();
        destination.IsTerminal = source.IsTerminal();
        destination.CapacityId = source.CapacityId;
        destination.BillId = source.BillId;
        destination.TourTitle = source.Tour?.Title;
        destination.TourStart = source.Tour?.TourStart;
        destination.TourEnd = source.Tour?.TourEnd;
        destination.TourStatus = source.Tour?.Status.ToString();
        destination.TourIsActive = source.Tour?.IsActive;

        return Task.CompletedTask;
    }
}

