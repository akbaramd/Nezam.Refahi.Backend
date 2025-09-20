using FluentValidation;
using Nezam.Refahi.Recreation.Application.Common.Validators;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.AddGuest;

public class AddGuestToReservationCommandValidator : AbstractValidator<AddGuestToReservationCommand>
{
    public AddGuestToReservationCommandValidator()
    {
        RuleFor(x => x.ReservationId)
            .NotEmpty()
            .WithMessage("شناسه رزرو الزامی است");

        RuleFor(x => x.Guest)
            .NotNull()
            .WithMessage("اطلاعات مهمان الزامی است")
            .SetValidator(new GuestParticipantDtoValidator());
    }
}