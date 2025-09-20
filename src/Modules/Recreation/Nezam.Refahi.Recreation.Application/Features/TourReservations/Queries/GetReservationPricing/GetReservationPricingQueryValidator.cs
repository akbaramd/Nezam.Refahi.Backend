using FluentValidation;

namespace Nezam.Refahi.Recreation.Application.Features.TourReservations.Queries.GetReservationPricing;

public class GetReservationPricingQueryValidator : AbstractValidator<GetReservationPricingQuery>
{
    public GetReservationPricingQueryValidator()
    {
        RuleFor(x => x.ReservationIdentifier)
            .NotEmpty()
            .WithMessage("شناسه رزرو یا کد پیگیری الزامی است")
            .Must(BeValidIdentifier)
            .WithMessage("شناسه رزرو باید یک GUID معتبر یا کد پیگیری باشد");
    }

    private static bool BeValidIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        // Check if it's a valid GUID
        if (Guid.TryParse(identifier, out _))
            return true;

        // Check if it's a valid tracking code (alphanumeric, minimum 6 characters)
        if (identifier.Length >= 6 && identifier.All(char.IsLetterOrDigit))
            return true;

        return false;
    }
}