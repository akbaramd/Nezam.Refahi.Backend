using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.HoldReservation;

/// <summary>
/// Validator for HoldReservationCommand
/// </summary>
public class HoldReservationCommandValidator : AbstractValidator<HoldReservationCommand>
{
    public HoldReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");
    }
}

