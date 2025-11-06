using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Contracts.Services;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ExpireReservation;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ExpireReservation;

/// <summary>
/// Handler for ExpireReservationCommand
/// Expires the reservation and cancels associated bills
/// </summary>
public class ExpireReservationCommandHandler : IRequestHandler<ExpireReservationCommand, ApplicationResult<ExpireReservationResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly IBillService _billService;
    private readonly ILogger<ExpireReservationCommandHandler> _logger;

    public ExpireReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        IRecreationUnitOfWork unitOfWork,
        IBillService billService,
        ILogger<ExpireReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _billService = billService ?? throw new ArgumentNullException(nameof(billService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<ExpireReservationResponse>> Handle(
        ExpireReservationCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (request.ReservationId == Guid.Empty)
                return ApplicationResult<ExpireReservationResponse>.Failure("شناسه رزرو معتبر نیست");

            // Load reservation
            var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
            if (reservation == null)
            {
                _logger.LogWarning("Reservation not found. ReservationId={ReservationId}", request.ReservationId);
                return ApplicationResult<ExpireReservationResponse>.Failure("رزرو مورد نظر یافت نشد");
            }

            // Check if already expired
            if (reservation.Status == ReservationStatus.Expired)
            {
                _logger.LogInformation("Reservation already expired. ReservationId={ReservationId}", request.ReservationId);
                return ApplicationResult<ExpireReservationResponse>.Success(new ExpireReservationResponse
                {
                    ReservationId = reservation.Id,
                    TrackingCode = reservation.TrackingCode,
                    PreviousStatus = ReservationStatus.Expired.ToString(),
                    BillCancelled = false,
                    BillId = reservation.BillId
                });
            }

            var previousStatus = reservation.Status;

            // Check if reservation can be expired (must be in a state that allows expiration)
            var canExpireStatuses = new[]
            {
                ReservationStatus.OnHold,
                ReservationStatus.Waitlisted
            };

            if (!canExpireStatuses.Contains(reservation.Status))
            {
                _logger.LogWarning("Cannot expire reservation in status {Status}. ReservationId={ReservationId}",
                    reservation.Status, request.ReservationId);
                return ApplicationResult<ExpireReservationResponse>.Failure(
                    $"امکان انقضای رزرو در وضعیت {reservation.Status} وجود ندارد");
            }

            // Begin transaction
            await _unitOfWork.BeginAsync(cancellationToken);

            try
            {
                // Mark reservation as expired
                reservation.MarkAsExpired();

                // If reservation has a bill, cancel it using BillService
                bool billCancelled = false;
                if (reservation.BillId.HasValue)
                {
                    _logger.LogInformation("Cancelling bill for expired reservation. ReservationId={ReservationId}, BillId={BillId}",
                        reservation.Id, reservation.BillId.Value);

                    var cancelBillResult = await _billService.CancelBillAsync(
                        billId: reservation.BillId.Value,
                        reason: request.Reason ?? "رزرو منقضی شد",
                        cancellationToken: cancellationToken);
                    
                    if (cancelBillResult.IsSuccess)
                    {
                        billCancelled = true;
                        _logger.LogInformation("Bill cancelled successfully. BillId={BillId}, ReservationId={ReservationId}",
                            reservation.BillId.Value, reservation.Id);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to cancel bill. BillId={BillId}, ReservationId={ReservationId}, Error={Error}",
                            reservation.BillId.Value, reservation.Id, cancelBillResult.Message ?? "Unknown error");
                        // Continue with expiration even if bill cancellation fails
                    }
                }

                // Update reservation
                await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation("Reservation expired successfully. ReservationId={ReservationId}, PreviousStatus={PreviousStatus}, BillCancelled={BillCancelled}",
                    reservation.Id, previousStatus, billCancelled);

                var response = new ExpireReservationResponse
                {
                    ReservationId = reservation.Id,
                    TrackingCode = reservation.TrackingCode,
                    PreviousStatus = previousStatus.ToString(),
                    BillCancelled = billCancelled,
                    BillId = reservation.BillId
                };

                return ApplicationResult<ExpireReservationResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error expiring reservation. ReservationId={ReservationId}", request.ReservationId);
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error expiring reservation. ReservationId={ReservationId}", request.ReservationId);
            return ApplicationResult<ExpireReservationResponse>.Failure(ex, "خطا در انقضای رزرو رخ داده است");
        }
    }
}

