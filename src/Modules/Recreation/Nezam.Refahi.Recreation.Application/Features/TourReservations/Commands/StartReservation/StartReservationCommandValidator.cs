using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Commands.StartReservation;

public class StartReservationCommandValidator : AbstractValidator<StartReservationCommand>
{
    public StartReservationCommandValidator()
    {
        RuleFor(x => x.TourId)
            .NotEmpty()
            .WithMessage("شناسه تور الزامی است");
        RuleFor(x => x. CapacityId)
          .NotEmpty()
          .WithMessage("شناسه ظرفیت الزامی است");

      
        RuleFor(x => x.UserNationalNumber)
          .NotEmpty()
          .WithMessage("کد ملی برای پاسخ مشخص شده الزامی است");
    }

    private static bool BeValidExpiryDate(DateTime? expiryDate)
    {
        return expiryDate == null || expiryDate.Value > DateTime.UtcNow;
    }
}
