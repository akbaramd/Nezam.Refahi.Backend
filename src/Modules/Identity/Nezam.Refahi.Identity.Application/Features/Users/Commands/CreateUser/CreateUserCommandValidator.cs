using FluentValidation;
using Nezam.Refahi.Identity.Domain.Repositories;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Identity.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{

    public CreateUserCommandValidator()
    {

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("نام اجباری است")
            .MinimumLength(2)
            .WithMessage("نام باید حداقل ۲ کاراکتر باشد")
            .MaximumLength(50)
            .WithMessage("نام نمی‌تواند بیشتر از ۵۰ کاراکتر باشد")
            .Matches(@"^[\u0600-\u06FF\u0750-\u077F\s]+$")
            .WithMessage("نام باید فقط شامل حروف فارسی باشد");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("نام خانوادگی اجباری است")
            .MinimumLength(2)
            .WithMessage("نام خانوادگی باید حداقل ۲ کاراکتر باشد")
            .MaximumLength(50)
            .WithMessage("نام خانوادگی نمی‌تواند بیشتر از ۵۰ کاراکتر باشد")
            .Matches(@"^[\u0600-\u06FF\u0750-\u077F\s]+$")
            .WithMessage("نام خانوادگی باید فقط شامل حروف فارسی باشد");

        RuleFor(x => x.PhoneNumber)
          .Cascade(CascadeMode.Stop)
          .NotEmpty().WithMessage("شماره موبایل اجباری است")
          // قبول هر دو فرم: 09xxxxxxxxx و +989xxxxxxxxx
          .Matches(@"^(?:09\d{9}|\+989\d{9})$")
          .WithMessage("شماره موبایل باید به فرم 09xxxxxxxxx یا +989xxxxxxxxx باشد");

        RuleFor(x => x.NationalId)
            .NotEmpty()
            .WithMessage("کد ملی اجباری است")
            .Length(10)
            .WithMessage("کد ملی باید دقیقاً ۱۰ رقم باشد")
            .Matches(@"^\d{10}$")
            .WithMessage("کد ملی باید فقط شامل اعداد باشد")
            .Must(BeValidNationalId)
            .WithMessage("کد ملی وارد شده معتبر نیست")
            ;
    }


  

    private static bool BeValidNationalId(string nationalId)
    {
        if (string.IsNullOrWhiteSpace(nationalId) || nationalId.Length != 10)
            return false;

        if (!nationalId.All(char.IsDigit))
            return false;

        // Check if all digits are the same (invalid cases)
        if (nationalId.All(c => c == nationalId[0]))
            return false;

        // Validate using Iranian national ID check digit algorithm
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += (nationalId[i] - '0') * (10 - i);
        }

        int remainder = sum % 11;
        int checkDigit = remainder < 2 ? remainder : 11 - remainder;

        return checkDigit == (nationalId[9] - '0');
    }
}