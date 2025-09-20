using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Payments;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.CancelPayment;

/// <summary>
/// Validator for CancelPaymentCommand
/// </summary>
public class CancelPaymentCommandValidator : AbstractValidator<CancelPaymentCommand>
{
    public CancelPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("Payment ID is required");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Cancellation reason cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Reason));
    }
}