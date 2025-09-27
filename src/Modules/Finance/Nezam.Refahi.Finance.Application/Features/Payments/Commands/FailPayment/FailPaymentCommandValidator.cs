using FluentValidation;
using Nezam.Refahi.Finance.Application.Commands.Payments;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.FailPayment;

/// <summary>
/// Validator for FailPaymentCommand
/// </summary>
public class FailPaymentCommandValidator : AbstractValidator<FailPaymentCommand>
{
    public FailPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("Payment ID is required");

        RuleFor(x => x.FailureReason)
            .MaximumLength(500)
            .WithMessage("Failure reason cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.FailureReason));
    }
}
