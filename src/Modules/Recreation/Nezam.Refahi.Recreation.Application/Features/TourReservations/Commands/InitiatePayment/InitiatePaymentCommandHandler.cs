using MediatR;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;
using Nezam.Refahi.Finance.Contracts.Dtos;
using Nezam.Refahi.Recreation.Application.Services;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Entities;
using Nezam.Refahi.Recreation.Domain.Enums;
using Nezam.Refahi.Recreation.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.InitiatePayment;

public class
  InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, ApplicationResult<InitiatePaymentResponse>>
{
  private readonly ITourReservationRepository _reservationRepository;
  private readonly ITourRepository _tourRepository;
  private readonly ITourPricingRepository _pricingRepository;
  private readonly IRecreationUnitOfWork _unitOfWork;
  private readonly IMediator _mediator;

  public InitiatePaymentCommandHandler(
    ITourReservationRepository reservationRepository,
    ITourRepository tourRepository,
    ITourPricingRepository pricingRepository,
    IRecreationUnitOfWork unitOfWork,
    IMediator mediator)
  {
    _reservationRepository = reservationRepository;
    _tourRepository = tourRepository;
    _pricingRepository = pricingRepository;
    _unitOfWork = unitOfWork;
    _mediator = mediator;
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

    // Create bill through Finance module
    var createBillCommand = new CreateBillCommand
    {
      Title = $"فاکتور تور {tour.Title}",
      ReferenceId = reservation.Id.ToString(),
      BillType = "TourReservation",
      UserNationalNumber = mainParticipant.NationalNumber,
      UserFullName = mainParticipant.FullName,
      Description = $"پرداخت رزرو تور {tour.Title} - کد پیگیری: {reservation.TrackingCode}",
      DueDate = reservation.ExpiryDate,
      Metadata = new Dictionary<string, string>
      {
        ["TourId"] = tour.Id.ToString(),
        ["ReservationId"] = reservation.Id.ToString(),
        ["TourTitle"] = tour.Title,
        ["ParticipantCount"] = reservation.GetParticipantCount().ToString(),
        ["ReservationDate"] = reservation.ReservationDate.ToString("yyyy-MM-dd HH:mm:ss")
      },
      Items = billItems
    };

    var billResult = await _mediator.Send(createBillCommand, cancellationToken);

    if (billResult.Data == null || !billResult.IsSuccess)
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure(
        billResult.Errors, $"خطا در ایجاد فاکتور: {billResult.Message}");
    }

    // Update reservation status and set bill ID
    try
    {
      var issueResult =
        await _mediator.Send(new IssueBillCommand() { BillId = billResult.Data.BillId }, cancellationToken);

      if (issueResult.Data == null || (!issueResult.IsSuccess))
      {
        var allErrors = new List<string>();
        allErrors.AddRange(billResult.Errors);
        allErrors.AddRange(issueResult.Errors);
        return ApplicationResult<InitiatePaymentResponse>.Failure(
          allErrors, $"خطا در تایید فاکتور: {billResult.Message} : {issueResult.Message}");
      }

      reservation.SetToPaying();
      reservation.SetBillId(billResult.Data!.BillId);

      await _reservationRepository.UpdateAsync(reservation, cancellationToken: cancellationToken);
      await _unitOfWork.SaveChangesAsync(cancellationToken);

      return ApplicationResult<InitiatePaymentResponse>.Success(new InitiatePaymentResponse
      {
        BillId = billResult.Data.BillId,
        BillNumber = billResult.Data.BillNumber,
        TotalAmountRials = totalAmount.AmountRials,
        PaymentUrl = GeneratePaymentUrl(billResult.Data.BillId),
        ExpiryDate = reservation.ExpiryDate ?? DateTime.UtcNow.AddMinutes(15)
      });
    }
    catch (Exception ex)
    {
      return ApplicationResult<InitiatePaymentResponse>.Failure(
        ex, $"خطا در به‌روزرسانی رزرو");
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

  private List<CreateBillItemDto> CreateBillItems(
    TourReservation reservation,
    Tour tour,
    List<TourPricing> pricing)
  {
    var items = new List<CreateBillItemDto>();

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

        items.Add(new CreateBillItemDto
        {
          Title = $"تور {tour.Title} - {typeDescription}",
          Description = $"{typeDescription} تور {tour.Title} ({count} نفر)",
          UnitPriceRials = applicablePricing.GetEffectivePrice().AmountRials,
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
