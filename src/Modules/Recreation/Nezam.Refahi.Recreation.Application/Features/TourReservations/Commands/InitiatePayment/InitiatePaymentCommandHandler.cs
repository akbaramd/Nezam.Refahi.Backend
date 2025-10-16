using MediatR;
using Nezam.Refahi.Recreation.Application.Dtos;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Contracts.IntegrationEvents;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Shared.Infrastructure.Outbox;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.InitiatePayment;

public class
  InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, ApplicationResult<InitiatePaymentResponse>>
{
  private readonly ITourReservationRepository _reservationRepository;
  private readonly ITourRepository _tourRepository;
  private readonly ITourPricingRepository _pricingRepository;
  private readonly IRecreationUnitOfWork _unitOfWork;
  private readonly IOutboxPublisher _outboxPublisher;

  public InitiatePaymentCommandHandler(
    ITourReservationRepository reservationRepository,
    ITourRepository tourRepository,
    ITourPricingRepository pricingRepository,
    IRecreationUnitOfWork unitOfWork,
    IOutboxPublisher outboxPublisher)
  {
    _reservationRepository = reservationRepository;
    _tourRepository = tourRepository;
    _pricingRepository = pricingRepository;
    _unitOfWork = unitOfWork;
    _outboxPublisher = outboxPublisher;
  }

  public async Task<ApplicationResult<InitiatePaymentResponse>> Handle(
    InitiatePaymentCommand request,
    CancellationToken cancellationToken)
  {
    // Get reservation with participants
    var reservation = await _reservationRepository.GetByIdAsync(request.ReservationId, cancellationToken);
    if (reservation == null)
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure("رزرو مورد نظر یافت نشد");
    }

    // Check if reservation is valid for payment
    if (reservation.Status != ReservationStatus.Held)
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure("وضعیت رزرو برای پرداخت مناسب نیست");
    }

    // Check expiry with buffer time for payment processing
    if (reservation.IsExpired())
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure("زمان رزرو منقضی شده است");
    }

    // Additional check: ensure there's enough time for payment processing
    if (reservation.ExpiryDate.HasValue && reservation.ExpiryDate.Value <= DateTime.UtcNow.AddMinutes(2))
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure("زمان باقی‌مانده برای پردازش پرداخت کافی نیست");
    }

    if (!reservation.Participants.Any())
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure("رزرو فاقد شرکت‌کننده است");
    }

    // Get tour details
    var tour = await _tourRepository.GetByIdAsync(reservation.TourId);
    if (tour == null)
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure("تور مورد نظر یافت نشد");
    }

    // Get pricing information
    var pricing = await _pricingRepository.GetByTourIdAsync(tour.Id, cancellationToken);
    if (pricing == null || !pricing.Any())
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure("اطلاعات قیمت‌گذاری تور یافت نشد");
    }

    // Calculate total amount
    var totalAmount = CalculateTotalAmount(reservation, pricing.ToList());

    // Create bill items for each participant
    var billItems = CreateBillItems(reservation, tour, pricing.ToList());

    // Get main participant for bill details
    var mainParticipant = reservation.GetMainParticipant();
    if (mainParticipant == null)
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure("شرکت‌کننده اصلی یافت نشد");
    }

    // Create integration event for bill creation
    var paymentRequestedEvent = new ReservationPaymentRequestedIntegrationEvent
    {
      ReservationId = reservation.Id,
      TrackingCode = reservation.TrackingCode,
      TourId = tour.Id,
      TourTitle = tour.Title,
      ReservationDate = reservation.ReservationDate,
      ExpiryDate = reservation.ExpiryDate,
      ExternalUserId = request.ExternalUserId,
      UserFullName = mainParticipant.FullName,
      TotalAmountRials = (long)totalAmount.AmountRials,
      BillTitle = $"فاکتور تور {tour.Title}",
      BillType = "TourReservation",
      Description = $"پرداخت رزرو تور {tour.Title} - کد پیگیری: {reservation.TrackingCode}",
      Metadata = new Dictionary<string, string>
      {
        ["TourId"] = tour.Id.ToString(),
        ["ReservationId"] = reservation.Id.ToString(),
        ["TrackingCode"] = reservation.TrackingCode,
        ["TourTitle"] = tour.Title,
        ["ParticipantCount"] = reservation.GetParticipantCount().ToString(),
        ["ReservationDate"] = reservation.ReservationDate.ToString("yyyy-MM-dd HH:mm:ss")
      },
      BillItems = billItems.Select(item => new ReservationBillItemDto
      {
        Title = item.Title,
        Description = item.Description,
        UnitPriceRials = item.UnitPriceRials,
        Quantity = item.Quantity,
        DiscountPercentage = item.DiscountPercentage
      }).ToList()
    };

    try
    {
      // Publish integration event through outbox
      await _outboxPublisher.PublishAsync(paymentRequestedEvent, cancellationToken);

      // Update reservation status to indicate payment is being processed
      reservation.SetToPaying();

      await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return ApplicationResult<InitiatePaymentResponse>.Success(new InitiatePaymentResponse
      {
        BillId = Guid.Empty, // Will be updated when bill is created
        BillNumber = string.Empty, // Will be updated when bill is created
        TotalAmountRials = totalAmount.AmountRials,
        PaymentUrl = GeneratePaymentUrl(reservation.Id), // Use reservation ID for now
        ExpiryDate = reservation.ExpiryDate ?? DateTime.UtcNow.AddMinutes(15)
      });
    }
    catch (Exception ex)
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure(
        ex, $"خطا در درخواست پرداخت");
    }
  }

  private Money CalculateTotalAmount(TourReservation reservation, List<TourPricing> pricing)
  {
    decimal totalRials = 0;

    foreach (var participant in reservation.Participants)
    {
      // Find applicable pricing for this participant
      var applicablePricing = pricing
        .Where(p => p.ParticipantType == participant.ParticipantType)
        .FirstOrDefault();

      if (applicablePricing != null)
      {
        totalRials += applicablePricing.GetEffectivePrice().AmountRials;
      }
    }

    return new Money(totalRials);
  }

  private List<ReservationBillItemDto> CreateBillItems(
    TourReservation reservation,
    Tour tour,
    List<TourPricing> pricing)
  {
    var items = new List<ReservationBillItemDto>();

    // Group participants by type to create consolidated items
    var participantGroups = reservation.Participants
      .GroupBy(p => p.ParticipantType)
      .ToList();

    foreach (var group in participantGroups)
    {
      var participantType = group.Key;
      var count = group.Count();

      // Find pricing for this participant type
      var applicablePricing = pricing
        .FirstOrDefault(p => p.ParticipantType == participantType);

      if (applicablePricing != null)
      {
        var typeDescription = GetParticipantTypeDescription(participantType);

        items.Add(new ReservationBillItemDto
        {
          Title = $"تور {tour.Title} - {typeDescription}",
          Description = $"{typeDescription} تور {tour.Title} ({count} نفر)",
          UnitPriceRials = (long)applicablePricing.GetEffectivePrice().AmountRials,
          Quantity = count,
          DiscountPercentage = null // No discount for now
        });
      }
    }

    return items;
  }

  private string GetParticipantTypeDescription(ParticipantType participantType)
  {
    return participantType switch
    {
      ParticipantType.Member => "شرکت‌کننده اصلی",
      ParticipantType.Guest => "همراه",
      _ => "شرکت‌کننده"
    };
  }

  private string GeneratePaymentUrl(Guid billId)
  {
    // This would be configured based on your payment gateway
    // For now, return a placeholder URL
    return $"/payment/pay/{billId}";
  }
}
