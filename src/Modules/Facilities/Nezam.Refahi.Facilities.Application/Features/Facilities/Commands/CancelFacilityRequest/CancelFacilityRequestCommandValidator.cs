using FluentValidation;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CancelFacilityRequest;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.CancelFacilityRequest;

/// <summary>
/// Validator for CancelFacilityRequestCommand
/// </summary>
public class CancelFacilityRequestCommandValidator : AbstractValidator<CancelFacilityRequestCommand>
{
    public CancelFacilityRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("شناسه درخواست الزامی است");

        RuleFor(x => x.Reason)
            .MaximumLength(1000)
            .WithMessage("دلیل لغو نمی‌تواند از ۱۰۰۰ کاراکتر بیشتر باشد")
            .When(x => !string.IsNullOrEmpty(x.Reason));

        RuleFor(x => x.CancelledByUserId)
            .NotEmpty()
            .WithMessage("شناسه لغوکننده الزامی است");
    }
}
