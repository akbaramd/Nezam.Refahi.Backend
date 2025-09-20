using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CancelReservation;

/// <summary>
/// Validator for CancelReservationCommand
/// </summary>
public class CancelReservationCommandValidator : AbstractValidator<CancelReservationCommand>
{
    public CancelReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Reason))
            .WithMessage("دلیل لغو نمی‌تواند بیشتر از 500 کاراکتر باشد");

        // Note: PermanentDelete is optional and defaults to true, so no validation needed
    }
}