using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ReactivateReservation;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ReactivateReservation;

public sealed class ReactivateReservationCommandHandler : IRequestHandler<ReactivateReservationCommand, ApplicationResult<ReactivateReservationResponse>>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ILogger<ReactivateReservationCommandHandler> _logger;

    public ReactivateReservationCommandHandler(
        ITourReservationRepository reservationRepository,
        IRecreationUnitOfWork unitOfWork,
        ILogger<ReactivateReservationCommandHandler> logger)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<ReactivateReservationResponse>> Handle(ReactivateReservationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.ReservationId == Guid.Empty)
                return ApplicationResult<ReactivateReservationResponse>.Failure("شناسه رزرو معتبر نیست");

            var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
            if (reservation == null)
                return ApplicationResult<ReactivateReservationResponse>.Failure("رزرو یافت نشد");


            if (reservation.Status != ReservationStatus.Expired)
                return ApplicationResult<ReactivateReservationResponse>.Failure($"امکان فعال‌سازی رزرو در وضعیت {reservation.Status} وجود ندارد");

            await _unitOfWork.BeginAsync(cancellationToken);
            try
            {
                reservation.ReactivateToDraft();

                await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                var response = new ReactivateReservationResponse
                {
                    ReservationId = reservation.Id,
                    Status = reservation.Status.ToString()
                };

                return ApplicationResult<ReactivateReservationResponse>.Success(response, "رزرو با موفقیت به پیش‌نویس بازگردانده شد");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error while reactivating reservation. ReservationId={ReservationId}", request.ReservationId);
                return ApplicationResult<ReactivateReservationResponse>.Failure(ex, "خطا در فعال‌سازی مجدد رزرو رخ داده است");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while reactivating reservation. ReservationId={ReservationId}", request.ReservationId);
            return ApplicationResult<ReactivateReservationResponse>.Failure(ex, "خطای غیرمنتظره در فعال‌سازی مجدد رزرو");
        }
    }
}


