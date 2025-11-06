using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;

namespace Nezam.Refahi.Recreation.Application.EventConsumers;

/// <summary>
/// Handles PaymentFailedIntegrationEvent to handle payment failures for tour reservations
/// When a payment fails, we may need to update reservation status or take other actions
/// </summary>
public class PaymentFailedEventConsumer : INotificationHandler<PaymentFailedIntegrationEvent>
{
    private readonly ITourReservationRepository _reservationRepository;
    private readonly IRecreationUnitOfWork _unitOfWork;
    private readonly ILogger<PaymentFailedEventConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public PaymentFailedEventConsumer(
        ITourReservationRepository reservationRepository,
        IRecreationUnitOfWork unitOfWork,
        ILogger<PaymentFailedEventConsumer> logger,
        IServiceProvider serviceProvider)
    {
        _reservationRepository = reservationRepository ?? throw new ArgumentNullException(nameof(reservationRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task Handle(PaymentFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("پردازش رویداد پرداخت ناموفق برای پرداخت {PaymentId}، شناسه رزرو: {ReferenceId}", 
                notification.PaymentId, notification.ReferenceId);

            // Check if this is a tour reservation payment
            if (notification.ReferenceType != "TourReservation")
            {
                _logger.LogDebug("نادیده گیری رویداد پرداخت ناموفق - نوع مرجع {ReferenceType} مربوط به رزرو تور نیست", 
                    notification.ReferenceType);
                return;
            }

            // Find reservation by tracking code (ReferenceId is now the tracking code)
            var reservation = await _reservationRepository.GetByTrackingCodeAsync(notification.ReferenceId, cancellationToken);
            
            if (reservation == null)
            {
                _logger.LogWarning("هیچ رزروی با کد پیگیری {TrackingCode} برای رویداد پرداخت ناموفق یافت نشد", 
                    notification.ReferenceId);
                return;
            }

            // Check if reservation is in Paying status
            if (reservation.Status != Nezam.Refahi.Recreation.Domain.Enums.ReservationStatus.PendingConfirmation)
            {
                _logger.LogInformation("رزرو {ReservationId} در وضعیت Paying نیست. وضعیت فعلی: {Status}. هیچ اقدامی لازم نیست.", 
                    reservation.Id, reservation.Status);
                return;
            }

            // Handle payment failure by updating reservation status
            await _unitOfWork.BeginAsync(cancellationToken);
            
            try
            {
                // Mark reservation as payment failed
                reservation.MarkPaymentFailed($"پرداخت ناموفق: {notification.FailureReason}");
                
                // Update reservation first
                await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);
                
                // Release capacity since payment failed
                if (reservation.CapacityId.HasValue)
                {
                    var capacityRepository = _serviceProvider.GetRequiredService<ITourCapacityRepository>();
                    var capacity = await capacityRepository.GetByIdAsync(reservation.CapacityId.Value, cancellationToken);
                    if (capacity != null)
                    {
                        var participantCount = reservation.GetParticipantCount();
                        capacity.ReleaseParticipants(participantCount);
                        
                        _logger.LogInformation("تعداد {Count} شرکت‌کننده از ظرفیت {CapacityId} آزاد شد (پرداخت ناموفق)", 
                            participantCount, capacity.Id);
                    }
                    else
                    {
                        _logger.LogWarning("ظرفیت {CapacityId} یافت نشد برای رزرو {ReservationId}", 
                            reservation.CapacityId.Value, reservation.Id);
                    }
                }
                
                // Save all changes
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
                
                _logger.LogInformation("پرداخت ناموفق برای رزرو {ReservationId} با موفقیت پردازش شد. وضعیت به PaymentFailed تغییر کرد", 
                    reservation.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "خطا در پردازش پرداخت ناموفق برای رزرو {ReservationId}", reservation.Id);
                throw;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در پردازش رویداد پرداخت ناموفق برای پرداخت {PaymentId}، کد پیگیری: {ReferenceId}", 
                notification.PaymentId, notification.ReferenceId);
            throw;
        }
    }
}

