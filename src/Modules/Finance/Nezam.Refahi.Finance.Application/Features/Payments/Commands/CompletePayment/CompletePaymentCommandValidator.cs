using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Payments;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.CompletePayment;

/// <summary>
/// Validator for CompletePaymentCommand
/// </summary>
public class CompletePaymentCommandValidator : AbstractValidator<CompletePaymentCommand>
{
    public CompletePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("Payment ID is required");

        RuleFor(x => x.GatewayTransactionId)
            .NotEmpty()
            .WithMessage("Gateway transaction ID is required")
            .MaximumLength(200)
            .WithMessage("Gateway transaction ID cannot exceed 200 characters");

        RuleFor(x => x.GatewayReference)
            .MaximumLength(200)
            .WithMessage("Gateway reference cannot exceed 200 characters")
            .When(x => !string.IsNullOrEmpty(x.GatewayReference));
    }
}