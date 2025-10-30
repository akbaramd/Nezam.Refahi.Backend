using FluentValidation;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycles;

/// <summary>
/// Validator for GetFacilityCyclesWithUserQuery
/// </summary>
public class GetFacilityCyclesWithUserQueryValidator : AbstractValidator<GetFacilityCyclesWithUserQuery>
{
    public GetFacilityCyclesWithUserQueryValidator()
    {
        RuleFor(x => x.FacilityId)
            .NotEmpty().WithMessage("شناسه تسهیلات الزامی است");

        RuleFor(x => x.NationalNumber)
            .Length(10).WithMessage("شماره ملی باید ۱۰ رقم باشد")
            .Matches("^[0-9]{10}$").WithMessage("شماره ملی باید فقط شامل اعداد باشد")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalNumber));

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("شماره صفحه باید بزرگتر از صفر باشد");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("اندازه صفحه باید بزرگتر از صفر باشد")
            .LessThanOrEqualTo(100).WithMessage("اندازه صفحه نمی‌تواند بیشتر از ۱۰۰ باشد");

        RuleFor(x => x.Status)
            .Must(BeValidStatus).WithMessage("وضعیت نامعتبر است")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));
    }

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return true;

        return status.ToLowerInvariant() switch
        {
            "draft" => true,
            "active" => true,
            "closed" => true,
            "completed" => true,
            "cancelled" => true,
            _ => false
        };
    }
}
