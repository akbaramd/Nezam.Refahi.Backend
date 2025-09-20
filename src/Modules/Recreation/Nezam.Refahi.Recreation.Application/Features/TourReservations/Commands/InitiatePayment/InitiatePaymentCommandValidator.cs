using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.InitiatePayment;

public class InitiatePaymentCommandValidator : AbstractValidator<InitiatePaymentCommand>
{
    public InitiatePaymentCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");

        RuleFor(x => x.PaymentMethod)
            .MaximumLength(50)
            .WithMessage("روش پرداخت نمی‌تواند بیش از ۵۰ کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.PaymentMethod));
    }
}