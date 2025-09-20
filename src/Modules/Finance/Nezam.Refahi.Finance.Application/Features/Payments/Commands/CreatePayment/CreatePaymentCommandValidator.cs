using FluentValidation;
using Nezam.Refahi.Finance.Contracts.Commands.Payments;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.CreatePayment;

/// <summary>
/// Validator for CreatePaymentCommand
/// </summary>
public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.BillId)
            .NotEmpty()
            .WithMessage("Bill ID is required");

        RuleFor(x => x.AmountRials)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than zero");

    

        RuleFor(x => x.PaymentMethod)
            .NotEmpty()
            .WithMessage("Payment method is required")
            .Must(BeValidPaymentMethod)
            .WithMessage("Invalid payment method. Valid values are: Online, Cash, Card");

        RuleFor(x => x.PaymentGateway)
            .Must(BeValidPaymentGateway)
            .WithMessage("Invalid payment gateway. Valid values are: Zarinpal, Mellat, Parsian, Pasargad")
            .When(x => !string.IsNullOrEmpty(x.PaymentGateway));

        RuleFor(x => x.CallbackUrl)
            .Must(BeValidUrl)
            .WithMessage("Invalid callback URL")
            .When(x => !string.IsNullOrEmpty(x.CallbackUrl));

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description cannot exceed 500 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Expiry date must be in the future")
            .When(x => x.ExpiryDate.HasValue);
    }

    private static bool BeValidPaymentMethod(string paymentMethod)
    {
        return Enum.TryParse<PaymentMethod>(paymentMethod, true, out _);
    }

    private static bool BeValidPaymentGateway(string? paymentGateway)
    {
        if (string.IsNullOrEmpty(paymentGateway))
            return true;

        return Enum.TryParse<PaymentGateway>(paymentGateway, true, out _);
    }

    private static bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}