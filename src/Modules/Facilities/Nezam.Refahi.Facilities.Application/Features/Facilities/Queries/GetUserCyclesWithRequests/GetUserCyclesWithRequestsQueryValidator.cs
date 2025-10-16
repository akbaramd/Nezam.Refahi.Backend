using FluentValidation;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Queries.GetUserCyclesWithRequests;

/// <summary>
/// Validator for GetUserCyclesWithRequestsQuery
/// </summary>
public class GetUserCyclesWithRequestsQueryValidator : AbstractValidator<GetUserCyclesWithRequestsQuery>
{
    public GetUserCyclesWithRequestsQueryValidator()
    {
        RuleFor(x => x.NationalNumber)
            .NotEmpty()
            .WithMessage("شماره ملی الزامی است")
            .Length(10)
            .WithMessage("شماره ملی باید ۱۰ رقم باشد")
            .Matches(@"^\d{10}$")
            .WithMessage("شماره ملی باید فقط شامل اعداد باشد");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("شماره صفحه باید بزرگتر از صفر باشد");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("اندازه صفحه باید بزرگتر از صفر باشد")
            .LessThanOrEqualTo(100)
            .WithMessage("اندازه صفحه نمی‌تواند بیشتر از ۱۰۰ باشد");

        RuleFor(x => x.RequestStatus)
            .Must(BeValidRequestStatus)
            .When(x => !string.IsNullOrWhiteSpace(x.RequestStatus))
            .WithMessage("وضعیت درخواست نامعتبر است");
    }

    private bool BeValidRequestStatus(string? status)
    {
        return !string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Enums.FacilityRequestStatus>(status, true, out _);
    }
}
