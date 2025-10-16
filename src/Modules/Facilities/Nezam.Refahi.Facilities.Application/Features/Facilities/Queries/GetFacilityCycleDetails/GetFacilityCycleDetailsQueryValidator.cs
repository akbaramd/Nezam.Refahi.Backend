using FluentValidation;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityCycleDetails;

/// <summary>
/// Validator for GetFacilityCycleDetailsQuery
/// </summary>
public class GetFacilityCycleDetailsQueryValidator : AbstractValidator<GetFacilityCycleDetailsQuery>
{
    public GetFacilityCycleDetailsQueryValidator()
    {
        RuleFor(x => x.CycleId)
            .NotEmpty().WithMessage("شناسه دوره تسهیلات الزامی است");

        RuleFor(x => x.NationalNumber)
            .Length(10).WithMessage("شماره ملی باید ۱۰ رقم باشد")
            .Matches("^[0-9]{10}$").WithMessage("شماره ملی باید فقط شامل اعداد باشد")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalNumber));
    }
}
