using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.FinalizeReservation;

/// <summary>
/// Validator for FinalizeReservationCommand
/// </summary>
public class FinalizeReservationCommandValidator : AbstractValidator<FinalizeReservationCommand>
{
    public FinalizeReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");
    }
}

