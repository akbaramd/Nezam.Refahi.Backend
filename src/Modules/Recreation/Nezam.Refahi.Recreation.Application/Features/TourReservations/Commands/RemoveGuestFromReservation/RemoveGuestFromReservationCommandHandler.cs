using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.RemoveGuest;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.RemoveGuest;

public sealed class RemoveGuestFromReservationCommandHandler
    : IRequestHandler<RemoveGuestFromReservationCommand, ApplicationResult<RemoveGuestFromReservationResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ILogger<RemoveGuestFromReservationCommandHandler> _logger;

    public RemoveGuestFromReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        ITourRepository tourRepository,
        IRecreationUnitOfWork unitOfWork,
        ILogger<RemoveGuestFromReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _tourRepository        = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _unitOfWork            = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger                = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<RemoveGuestFromReservationResponse>> Handle(
        RemoveGuestFromReservationCommand request,
        CancellationToken ct)
    {
        if (request.ReservationId == Guid.Empty)
            return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("شناسه رزرو معتبر نیست");
        if (request.ParticipantId == Guid.Empty)
            return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("شناسه شرکت‌کننده معتبر نیست");
        if (request.ExternalUserId == Guid.Empty)
            return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("شناسه کاربر الزامی است");

        try
        {
            // Load reservation (Participants must be available)
            var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, ct);
            if (reservation is null)
                return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("رزرو یافت نشد");


            // Validate reservation allows participant removal (domain behavior)
            if (!reservation.CanRemoveParticipant(out var canRemoveError))
                return ApplicationResult<RemoveGuestFromReservationResponse>.Failure(canRemoveError!);

            // Load tour for pricing recomputation and registration window checks
            var tour = await _tourRepository.FindOneAsync(t => t.Id == reservation.TourId, ct);
            if (tour is null)
                return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("تور مرتبط با رزرو یافت نشد");

            if (!tour.IsActive || !tour.IsRegistrationOpen(DateTime.UtcNow))
                return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("ثبت‌نام برای این تور مجاز نیست");

            // Find participant
            var participant = reservation.Participants.FirstOrDefault(p => p.Id == request.ParticipantId);
            if (participant is null)
                return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("شرکت‌کننده مورد نظر در این رزرو یافت نشد");

            // Only guests can be removed via this command. Prevent removing member/owner.
            if (participant.ParticipantType != ParticipantType.Guest)
                return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("حذف شرکت‌کننده عضو از این مسیر مجاز نیست");

            // Transactional remove
            await _unitOfWork.BeginAsync(ct);

            reservation.RemoveParticipant(participant.Id);    // domain method should enforce invariants

            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);

            // Recalculate totals (Money-safe)
            var pricing = tour.GetActivePricing();
            var guestPrice  = pricing.FirstOrDefault(p => p.ParticipantType == ParticipantType.Guest )?.GetEffectivePrice() ?? Money.Zero;
            var memberPrice = pricing.FirstOrDefault(p => p.ParticipantType == ParticipantType.Member)?.GetEffectivePrice() ?? Money.Zero;

            var memberCount = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Member);
            var guestCount  = reservation.Participants.Count(p => p.ParticipantType == ParticipantType.Guest);

            var updatedTotalRials = (memberCount * memberPrice.AmountRials) + (guestCount * guestPrice.AmountRials);

            var response = new RemoveGuestFromReservationResponse
            {
                ReservationId      = reservation.Id,
                CapacityId = reservation.CapacityId,
            };

            _logger.LogInformation("Removed guest {ParticipantId} from reservation {ReservationId}", participant.Id, reservation.Id);
            return ApplicationResult<RemoveGuestFromReservationResponse>.Success(response);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex, "Concurrency while removing guest. ReservationId={ReservationId}, ParticipantId={ParticipantId}",
                request.ReservationId, request.ParticipantId);
            try { await _unitOfWork.RollbackAsync(ct); } catch { /* ignore */ }
            return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("تداخل همزمانی رخ داد. دوباره تلاش کنید");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing guest. ReservationId={ReservationId}, ParticipantId={ParticipantId}",
                request.ReservationId, request.ParticipantId);
            try { await _unitOfWork.RollbackAsync(ct); } catch { /* ignore */ }
            return ApplicationResult<RemoveGuestFromReservationResponse>.Failure("خطا در حذف مهمان از رزرو");
        }
    }
}
