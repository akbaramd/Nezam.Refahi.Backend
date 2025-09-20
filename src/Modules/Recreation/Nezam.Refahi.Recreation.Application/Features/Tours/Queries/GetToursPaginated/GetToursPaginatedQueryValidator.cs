using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.Tours.Queries.GetToursPaginated;

/// <summary>
/// Validator for GetToursPaginatedQuery
/// </summary>
public class GetToursPaginatedQueryValidator : AbstractValidator<GetToursPaginatedQuery>
{
    public GetToursPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("شماره صفحه باید بزرگتر از صفر باشد");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("تعداد رکورد در هر صفحه باید بزرگتر از صفر باشد")
            .LessThanOrEqualTo(100)
            .WithMessage("تعداد رکورد در هر صفحه نمی‌تواند بیشتر از ۱۰۰ باشد");

        RuleFor(x => x.Search)
            .MaximumLength(200)
            .WithMessage("عبارت جستجو نمی‌تواند بیشتر از ۲۰۰ کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.Search));

     
    }
}