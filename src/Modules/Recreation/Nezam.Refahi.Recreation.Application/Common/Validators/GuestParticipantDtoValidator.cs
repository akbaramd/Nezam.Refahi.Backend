using FluentValidation;
using Nezam.Refahi.Recreation.Contracts.Dtos;
using Nezam.Refahi.Recreation.Domain.Enums;

namespace Nezam.Refahi.Recreation.Application.Common.Validators;

/// <summary>
/// Validator for GuestParticipantDto
/// </summary>
public class GuestParticipantDtoValidator : AbstractValidator<GuestParticipantDto>
{
    public GuestParticipantDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("نام الزامی است")
            .MaximumLength(100)
            .WithMessage("نام نمی‌تواند بیش از 100 کاراکتر باشد");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("نام خانوادگی الزامی است")
            .MaximumLength(100)
            .WithMessage("نام خانوادگی نمی‌تواند بیش از 100 کاراکتر باشد");

        RuleFor(x => x.NationalNumber)
            .NotEmpty()
            .WithMessage("کد ملی الزامی است")
            .Length(10)
            .WithMessage("کد ملی باید 10 رقم باشد")
            .Must(BeValidNationalNumber)
            .WithMessage("کد ملی وارد شده معتبر نیست");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("شماره تلفن الزامی است")
            .Matches(@"^09\d{9}$")
            .WithMessage("شماره تلفن باید به صورت 09xxxxxxxxx باشد");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("آدرس ایمیل معتبر نیست");

        RuleFor(x => x.BirthDate)
            .Must(BeValidBirthDate)
            .WithMessage("تاریخ تولد باید در گذشته باشد");

        RuleFor(x => x.EmergencyContactName)
            .MaximumLength(200)
            .WithMessage("نام مخاطب اضطراری نمی‌تواند بیش از 200 کاراکتر باشد");

        RuleFor(x => x.EmergencyContactPhone)
            .Matches(@"^09\d{9}$")
            .When(x => !string.IsNullOrWhiteSpace(x.EmergencyContactPhone))
            .WithMessage("شماره تلفن مخاطب اضطراری باید به صورت 09xxxxxxxxx باشد");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("یادداشت نمی‌تواند بیش از 1000 کاراکتر باشد");

        RuleFor(x => x.ParticipantType)
            .IsInEnum()
            .WithMessage("نوع شرکت‌کننده معتبر نیست");
    }

    private static bool BeValidNationalNumber(string nationalNumber)
    {
        if (string.IsNullOrWhiteSpace(nationalNumber) || nationalNumber.Length != 10)
            return false;

        if (!nationalNumber.All(char.IsDigit))
            return false;

        // Check for invalid patterns
        if (nationalNumber.Distinct().Count() == 1)
            return false;

        // Calculate check digit
        var sum = 0;
        for (var i = 0; i < 9; i++)
        {
            sum += int.Parse(nationalNumber[i].ToString()) * (10 - i);
        }

        var remainder = sum % 11;
        var checkDigit = int.Parse(nationalNumber[9].ToString());

        return remainder < 2 ? checkDigit == remainder : checkDigit == 11 - remainder;
    }

    private static bool BeValidBirthDate(DateTime birthDate)
    {
        return birthDate < DateTime.UtcNow.Date;
    }

    
}
