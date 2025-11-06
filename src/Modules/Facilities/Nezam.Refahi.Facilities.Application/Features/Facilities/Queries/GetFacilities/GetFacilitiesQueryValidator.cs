using FluentValidation;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilities;

/// <summary>
/// Validator for GetFacilitiesQuery
/// </summary>
public class GetFacilitiesQueryValidator : AbstractValidator<GetFacilitiesQuery>
{
    public GetFacilitiesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("شماره صفحه باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(1000)
            .WithMessage("شماره صفحه نمی‌تواند از ۱۰۰۰ بیشتر باشد");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("اندازه صفحه باید بیشتر از صفر باشد")
            .LessThanOrEqualTo(100)
            .WithMessage("اندازه صفحه نمی‌تواند از ۱۰۰ بیشتر باشد");

      

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .WithMessage("عبارت جستجو نمی‌تواند از ۲۰۰ کاراکتر بیشتر باشد")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));
    }
}
