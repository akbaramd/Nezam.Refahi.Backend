using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.CreateReservation;

public class CreateTourReservationCommandValidator : AbstractValidator<CreateTourReservationCommand>
{
    public CreateTourReservationCommandValidator()
    {
        RuleFor(x => x.TourId)
            .NotEmpty()
            .WithMessage("شناسه تور الزامی است");

      


        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("یادداشت نمی‌تواند بیش از 1000 کاراکتر باشد");
    }

    private static bool BeValidExpiryDate(DateTime? expiryDate)
    {
        return expiryDate == null || expiryDate.Value > DateTime.UtcNow;
    }
}
