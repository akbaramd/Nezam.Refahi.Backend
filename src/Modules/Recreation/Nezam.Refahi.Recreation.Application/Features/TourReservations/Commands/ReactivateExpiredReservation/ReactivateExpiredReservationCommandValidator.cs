using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.ReactivateExpiredReservation;

/// <summary>
/// Validator for ReactivateExpiredReservationCommand
/// </summary>
public class ReactivateExpiredReservationCommandValidator : AbstractValidator<ReactivateExpiredReservationCommand>
{
    public ReactivateExpiredReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Reason))
            .WithMessage("دلیل فعال‌سازی مجدد نمی‌تواند بیشتر از 500 کاراکتر باشد");

    }
}
