using FluentValidation;
using Nezam.Refahi.Surveying.Contracts.Queries;

namespace Nezam.Refahi.Surveying.Application.Features.Questions.Queries;

/// <summary>
/// Validator for GetSpecificQuestionQuery
/// </summary>
public class GetSpecificQuestionQueryValidator : AbstractValidator<GetSpecificQuestionQuery>
{
    public GetSpecificQuestionQueryValidator()
    {
        RuleFor(x => x.SurveyId)
            .NotEmpty()
            .WithMessage("شناسه نظرسنجی الزامی است");

        RuleFor(x => x.QuestionIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("ایندکس سوال باید بزرگتر یا مساوی صفر باشد");

        RuleFor(x => x.UserNationalNumber)
            .NotEmpty()
            .When(x => x.ResponseId.HasValue)
            .WithMessage("شناسه عضو برای پاسخ مشخص شده الزامی است");

        RuleFor(x => x.ResponseId)
            .NotEmpty()
            .WithMessage("شناسه پاسخ برای عضو مشخص شده الزامی است");
    }
}
