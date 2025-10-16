using FluentValidation;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.ApproveFacilityRequest;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.ApproveFacilityRequest;

/// <summary>
/// Validator for ApproveFacilityRequestCommand
/// </summary>
public class ApproveFacilityRequestCommandValidator : AbstractValidator<ApproveFacilityRequestCommand>
{
    public ApproveFacilityRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("شناسه درخواست الزامی است");

        RuleFor(x => x.ApprovedAmountRials)
            .GreaterThan(0)
            .WithMessage("مبلغ تأیید شده باید بیشتر از صفر باشد")
            .LessThan(1_000_000_000) // 1 billion Rials max
            .WithMessage("مبلغ تأیید شده نمی‌تواند از یک میلیارد ریال بیشتر باشد");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("واحد پول الزامی است")
            .Length(3)
            .WithMessage("کد واحد پول باید سه حرف باشد")
            .Matches("^[A-Z]{3}$")
            .WithMessage("کد واحد پول باید سه حرف بزرگ انگلیسی باشد");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("یادداشت‌ها نمی‌تواند از ۱۰۰۰ کاراکتر بیشتر باشد")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.ApproverUserId)
            .NotEmpty()
            .WithMessage("شناسه تأییدکننده الزامی است");
    }
}
