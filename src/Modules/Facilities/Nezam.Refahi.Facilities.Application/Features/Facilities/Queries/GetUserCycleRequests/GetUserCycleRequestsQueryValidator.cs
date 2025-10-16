using FluentValidation;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCycleRequests;

/// <summary>
/// Validator for GetUserCycleRequestsQuery
/// </summary>
public class GetUserCycleRequestsQueryValidator : AbstractValidator<GetUserCycleRequestsQuery>
{
    public GetUserCycleRequestsQueryValidator()
    {
        RuleFor(x => x.NationalNumber)
            .NotEmpty().WithMessage("شماره ملی الزامی است")
            .Length(10).WithMessage("شماره ملی باید ۱۰ رقم باشد")
            .Matches("^[0-9]{10}$").WithMessage("شماره ملی باید فقط شامل اعداد باشد");

        RuleFor(x => x.Page)
            .GreaterThan(0).WithMessage("شماره صفحه باید بزرگتر از صفر باشد");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("اندازه صفحه باید بزرگتر از صفر باشد")
            .LessThanOrEqualTo(100).WithMessage("اندازه صفحه نمی‌تواند بیشتر از ۱۰۰ باشد");

        RuleFor(x => x.Status)
            .Must(BeValidStatus).WithMessage("وضعیت درخواست نامعتبر است")
            .When(x => !string.IsNullOrWhiteSpace(x.Status));

        RuleFor(x => x.DateFrom)
            .LessThanOrEqualTo(x => x.DateTo)
            .WithMessage("تاریخ شروع نمی‌تواند بعد از تاریخ پایان باشد")
            .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
    }

    private static bool BeValidStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return true;

        return status.ToLowerInvariant() switch
        {
            "submitted" => true,
            "pendingdocuments" => true,
            "waitlisted" => true,
            "returnedforamendment" => true,
            "underreview" => true,
            "approved" => true,
            "rejected" => true,
            "cancelled" => true,
            "queuedfordispatch" => true,
            "senttobank" => true,
            "bankscheduled" => true,
            "processedbybank" => true,
            "completed" => true,
            "disbursed" => true,
            "expired" => true,
            "bankcancelled" => true,
            _ => false
        };
    }
}
