using FluentValidation;
using Nezam.Refahi.Surveying.Contracts.Queries;

namespace Nezam.Refahi.Surveying.Application.Features.Questions.Queries;

/// <summary>
/// Validator for GetPreviousQuestionsQuery
/// </summary>
public class GetPreviousQuestionsQueryValidator : AbstractValidator<GetPreviousQuestionsQuery>
{
    public GetPreviousQuestionsQueryValidator()
    {
        RuleFor(x => x.SurveyId)
            .NotEmpty()
            .WithMessage("شناسه نظرسنجی الزامی است");

        RuleFor(x => x.CurrentQuestionIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("ایندکس سوال فعلی باید بزرگتر یا مساوی صفر باشد");

        RuleFor(x => x.MaxCount)
            .GreaterThan(0)
            .LessThanOrEqualTo(50)
            .WithMessage("تعداد حداکثر سوالات باید بین 1 و 50 باشد");

        RuleFor(x => x.UserNationalNumber)
            .NotEmpty()
            .WithMessage("شماره ملی الزامی است")
            .Length(10)
            .WithMessage("شماره ملی باید ۱۰ رقم باشد")
            .Matches(@"^\d{10}$")
            .WithMessage("شماره ملی باید فقط شامل اعداد باشد");
    }
}
