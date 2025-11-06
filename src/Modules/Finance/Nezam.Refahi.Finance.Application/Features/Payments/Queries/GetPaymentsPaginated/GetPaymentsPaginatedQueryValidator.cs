using FluentValidation;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetPaymentsPaginated;

public class GetPaymentsPaginatedQueryValidator : AbstractValidator<GetPaymentsPaginatedQuery>
{
    public GetPaymentsPaginatedQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0)
            .WithMessage("شماره صفحه باید بزرگتر از صفر باشد");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("تعداد رکورد در هر صفحه باید بین 1 تا 100 باشد");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("تاریخ شروع باید کوچکتر یا مساوی تاریخ پایان باشد");
    }
}

