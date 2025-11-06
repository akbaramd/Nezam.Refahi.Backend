using FluentValidation;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CreateFacilityRequest;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CreateFacilityRequest;

/// <summary>
/// Validator for CreateFacilityRequestCommand
/// </summary>
public class CreateFacilityRequestCommandValidator : AbstractValidator<CreateFacilityRequestCommand>
{
    public CreateFacilityRequestCommandValidator()
    {
        RuleFor(x => x.FacilityCycleId)
            .NotEmpty()
            .WithMessage("شناسه دوره تسهیلات الزامی است");

        RuleFor(x => x.NationalNumber)
            .NotEmpty()
            .WithMessage("شماره ملی الزامی است")
            .Length(10)
            .WithMessage("شماره ملی باید ۱۰ رقم باشد")
            .Matches("^[0-9]{10}$")
            .WithMessage("شماره ملی باید فقط شامل اعداد باشد");

        RuleFor(x => x.PriceOptionId)
            .NotEmpty()
            .WithMessage("شناسه گزینه قیمت الزامی است");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("توضیحات نمی‌تواند از ۱۰۰۰ کاراکتر بیشتر باشد")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.IdempotencyKey)
            .MaximumLength(100)
            .WithMessage("کلید تکرار نمی‌تواند از ۱۰۰ کاراکتر بیشتر باشد")
            .When(x => !string.IsNullOrEmpty(x.IdempotencyKey));

        RuleFor(x => x.Metadata)
            .Must(BeValidMetadata)
            .WithMessage("اطلاعات اضافی نامعتبر است")
            .When(x => x.Metadata != null);
    }

    private static bool BeValidMetadata(Dictionary<string, string>? metadata)
    {
        if (metadata == null) return true;

        // Check total metadata size
        var totalSize = metadata.Sum(kvp => kvp.Key.Length + kvp.Value.Length);
        if (totalSize > 2000) return false;

        // Check individual key/value lengths
        return metadata.All(kvp => 
            kvp.Key.Length <= 50 && 
            kvp.Value.Length <= 500 &&
            !string.IsNullOrWhiteSpace(kvp.Key));
    }
}
