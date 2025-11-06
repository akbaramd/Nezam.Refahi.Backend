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
using Nezam.Refahi.Recreation.Application.Services; // IRecreationUnitOfWork
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ChangeReservationCapacity;

public sealed class ChangeReservationCapacityCommandHandler
    : IRequestHandler<ChangeReservationCapacityCommand, ApplicationResult<ChangeReservationCapacityCommandResult>>
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ILogger<ChangeReservationCapacityCommandHandler> _logger;

    public ChangeReservationCapacityCommandHandler(
        ITourRepository tourRepository,
        ITourReservationRepository reservationRepository,
        IRecreationUnitOfWork unitOfWork,
        ILogger<ChangeReservationCapacityCommandHandler> logger)
    {
        _tourRepository       = tourRepository       ?? throw new ArgumentNullException(nameof(tourRepository));
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _unitOfWork           = unitOfWork           ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger               = logger               ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<ChangeReservationCapacityCommandResult>> Handle(
        ChangeReservationCapacityCommand request,
        CancellationToken ct)
    {
        if (request.ReservationId == Guid.Empty)
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure("شناسه رزرو معتبر نیست");
        if (request.NewCapacityId == Guid.Empty)
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure("شناسه ظرفیت معتبر نیست");
        if (request.ExternalUserId == Guid.Empty)
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure("شناسه کاربر الزامی است");

        var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, ct);
        if (reservation is null)
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure("رزرو یافت نشد");


        // Validate reservation allows capacity change (domain behavior)
        if (!reservation.CanChangeCapacity(out var canChangeCapacityError))
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure(canChangeCapacityError!);

        // Load tour with capacities
        var tour = await _tourRepository.FindOneAsync(t => t.Id == reservation.TourId, ct);
        if (tour is null)
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure("تور یافت نشد");
        if (!tour.IsActive || !tour.IsRegistrationOpen(DateTime.UtcNow))
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure("ثبت‌نام برای این تور مجاز نیست");

        // Validate capacity belongs to this tour
        var newCapacity = tour.Capacities?.FirstOrDefault(c => c.Id == request.NewCapacityId);
        if (newCapacity is null)
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure("ظرفیت انتخاب‌شده متعلق به این تور نیست");

    
        try
        {
            await _unitOfWork.BeginAsync(ct);

            // No allocation/release here by requirement.
            // We only switch the capacity reference while still in Draft.
            reservation.SetCapacity(newCapacity.Id);

            await _reservationRepository.UpdateAsync(reservation, cancellationToken:ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await _unitOfWork.CommitAsync(ct);

            _logger.LogInformation(
                "Reservation capacity changed. ReservationId={ReservationId}, CapacityId={CapacityId}",
                reservation.Id, newCapacity.Id);

            return ApplicationResult<ChangeReservationCapacityCommandResult>.Success(new ChangeReservationCapacityCommandResult
            {
                ReservationId = reservation.Id,
                CapacityId    = newCapacity.Id
            });
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogWarning(ex,
                "Concurrency conflict while changing capacity. ReservationId={ReservationId}, CapacityId={CapacityId}",
                reservation.Id, newCapacity.Id);
            try { await _unitOfWork.RollbackAsync(ct); } catch { /* ignore */ }
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure("تداخل همزمانی رخ داد. فهرست ظرفیت‌ها را به‌روزرسانی کنید و مجدداً تلاش کنید");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to change reservation capacity. ReservationId={ReservationId}, CapacityId={CapacityId}",
                reservation.Id, newCapacity.Id);
            try { await _unitOfWork.RollbackAsync(ct); } catch { /* ignore */ }
            return ApplicationResult<ChangeReservationCapacityCommandResult>.Failure("تغییر ظرفیت با خطا مواجه شد");
        }
    }
}
