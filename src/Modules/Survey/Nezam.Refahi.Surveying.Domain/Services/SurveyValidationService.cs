using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Domain.Services;

/// <summary>
/// Domain service for survey response validation
/// Single responsibility: validating response completeness and correctness
/// </summary>
public class SurveyValidationService
{
    /// <summary>
    /// بررسی تکمیل بودن پاسخ بر اساس الزامات نظرسنجی
    /// </summary>
    public bool IsResponseComplete(Survey survey, Response response)
    {
        var requiredQuestions = survey.Questions.Where(q => q.IsRequired).ToList();
        
        foreach (var question in requiredQuestions)
        {
            if (!response.HasAnswerForQuestion(question.Id))
                return false;
        }

        return true;
    }

    /// <summary>
    /// بررسی اینکه آیا پاسخ Submitted است
    /// </summary>
    public bool IsResponseSubmitted(Response response)
    {
        return response.AttemptStatus == AttemptStatus.Submitted;
    }

    /// <summary>
    /// اعتبارسنجی پاسخ‌ها بر اساس محدودیت‌های سوالات
    /// </summary>
    public bool ValidateResponseAnswers(Survey survey, Response response)
    {
        foreach (var question in survey.Questions)
        {
            var questionAnswer = response.GetQuestionAnswer(question.Id);
            
            // Skip unanswered optional questions
            if (questionAnswer == null || !questionAnswer.HasAnswer())
            {
                if (question.IsRequired)
                    return false; // Required question not answered
                continue;
            }

            // Validate selected options
            var selectedOptions = questionAnswer.GetSelectedOptionIds();
            if (!question.ValidateSelectedOptions(selectedOptions))
                return false;

            // Validate text answers for textual questions
            if (question.Kind == QuestionKind.Textual)
            {
                if (!questionAnswer.HasTextAnswer)
                    return false;
            }
        }

        return true;
    }
}
