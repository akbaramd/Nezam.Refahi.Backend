using FluentValidation;
using Nezam.Refahi.Surveying.Contracts.Commands;

namespace Nezam.Refahi.Surveying.Application.Features.Responses.Validators;

/// <summary>
/// Validator for StartSurveyResponseCommand
/// </summary>
public class StartSurveyResponseCommandValidator : AbstractValidator<StartSurveyResponseCommand>
{
    public StartSurveyResponseCommandValidator()
    {
        RuleFor(x => x.SurveyId)
            .NotEmpty()
            .WithMessage("شناسه نظرسنجی الزامی است");

        RuleFor(x => x.NationalNumber)
            .NotEmpty()
            .WithMessage("شناسه کاربر الزامی است");

        RuleFor(x => x.ParticipantHash)
            .MaximumLength(100)
            .WithMessage("هش شرکت‌کننده نمی‌تواند بیش از 100 کاراکتر باشد")
            .When(x => !string.IsNullOrEmpty(x.ParticipantHash));

        RuleFor(x => x.DemographyData)
            .Must(data => data == null || data.Count <= 50)
            .WithMessage("حداکثر 50 فیلد دموگرافی مجاز است")
            .When(x => x.DemographyData != null);

        RuleForEach(x => x.DemographyData)
            .Must(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
            .WithMessage("کلید و مقدار دموگرافی نمی‌تواند خالی باشد")
            .When(x => x.DemographyData != null);
    }
}
