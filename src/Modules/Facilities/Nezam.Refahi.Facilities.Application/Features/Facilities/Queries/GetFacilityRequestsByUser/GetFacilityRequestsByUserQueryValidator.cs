using FluentValidation;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetFacilityRequests;

/// <summary>
/// Validator for GetFacilityRequestsByUserQuery
/// </summary>
public class GetFacilityRequestsQueryValidator : AbstractValidator<GetFacilityRequestsByUserQuery>
{
    public GetFacilityRequestsQueryValidator()
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

        RuleFor(x => x.Status)
            .MaximumLength(50)
            .WithMessage("وضعیت درخواست نمی‌تواند از ۵۰ کاراکتر بیشتر باشد")
            .When(x => !string.IsNullOrEmpty(x.Status));

        RuleFor(x => x.SearchTerm)
            .MaximumLength(200)
            .WithMessage("عبارت جستجو نمی‌تواند از ۲۰۰ کاراکتر بیشتر باشد")
            .When(x => !string.IsNullOrEmpty(x.SearchTerm));

        RuleFor(x => x.DateFrom)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("تاریخ شروع نمی‌تواند در آینده باشد")
            .When(x => x.DateFrom.HasValue);

        RuleFor(x => x.DateTo)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("تاریخ پایان نمی‌تواند در آینده باشد")
            .When(x => x.DateTo.HasValue);

        RuleFor(x => x)
            .Must(HaveValidDateRange)
            .WithMessage("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد")
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
    }

    private static bool HaveValidDateRange(GetFacilityRequestsByUserQuery byUserQuery)
    {
        if (!byUserQuery.DateFrom.HasValue || !byUserQuery.DateTo.HasValue)
            return true;

        return byUserQuery.DateFrom.Value <= byUserQuery.DateTo.Value;
    }
}
