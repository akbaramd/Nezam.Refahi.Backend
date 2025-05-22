using FluentValidation;
using System.Text.RegularExpressions;

namespace Nezam.Refahi.Application.Features.Auth.Commands.CompleteRegistration;

/// <summary>
/// Validator for the CompleteRegistrationCommand
/// </summary>
public class CompleteRegistrationCommandValidator : AbstractValidator<CompleteRegistrationCommand>
{
    public CompleteRegistrationCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");
            
        RuleFor(x => x.NationalId)
            .NotEmpty().WithMessage("National ID is required.")
            .Length(10).WithMessage("National ID must be exactly 10 digits.")
            .Matches(new Regex("^[0-9]{10}$")).WithMessage("National ID must contain only digits.");
            

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}
