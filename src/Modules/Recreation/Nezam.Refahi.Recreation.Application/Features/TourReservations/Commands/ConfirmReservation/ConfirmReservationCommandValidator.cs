using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ConfirmReservation;

/// <summary>
/// Validator for ConfirmReservationCommand
/// </summary>
public class ConfirmReservationCommandValidator : AbstractValidator<ConfirmReservationCommand>
{
    public ConfirmReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");

        RuleFor(x => x.TotalAmountRials)
            .GreaterThan(0)
            .When(x => x.TotalAmountRials.HasValue)
            .WithMessage("مبلغ کل باید بیشتر از صفر باشد");

        RuleFor(x => x.PaymentReference)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.PaymentReference))
            .WithMessage("شماره مرجع پرداخت نمی‌تواند بیشتر از 100 کاراکتر باشد");

       
    }
}