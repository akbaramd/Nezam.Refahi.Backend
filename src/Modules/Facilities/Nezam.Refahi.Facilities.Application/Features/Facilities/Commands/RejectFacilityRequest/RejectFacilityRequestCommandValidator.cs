using FluentValidation;
using Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.RejectFacilityRequest;

namespace Nezam.Refahi.Facilities.Application.Features.Facilities.Commands.RejectFacilityRequest;

/// <summary>
/// Validator for RejectFacilityRequestCommand
/// </summary>
public class RejectFacilityRequestCommandValidator : AbstractValidator<RejectFacilityRequestCommand>
{
    public RejectFacilityRequestCommandValidator()
    {
        RuleFor(x => x.RequestId)
            .NotEmpty()
            .WithMessage("شناسه درخواست الزامی است");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("دلیل رد درخواست الزامی است")
            .MinimumLength(10)
            .WithMessage("دلیل رد درخواست باید حداقل ۱۰ کاراکتر باشد")
            .MaximumLength(1000)
            .WithMessage("دلیل رد درخواست نمی‌تواند از ۱۰۰۰ کاراکتر بیشتر باشد");

        RuleFor(x => x.RejectorUserId)
            .NotEmpty()
            .WithMessage("شناسه ردکننده الزامی است");
    }
}
