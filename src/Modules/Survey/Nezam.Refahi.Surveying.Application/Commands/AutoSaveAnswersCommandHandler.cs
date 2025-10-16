using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Commands;

/// <summary>
/// Handler for AutoSaveAnswersCommand (C8)
/// </summary>
public class AutoSaveAnswersCommandHandler : IRequestHandler<AutoSaveAnswersCommand, ApplicationResult<AutoSaveAnswersResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ISurveyUnitOfWork _unitOfWork;
    private readonly ILogger<AutoSaveAnswersCommandHandler> _logger;

    public AutoSaveAnswersCommandHandler(
        ISurveyRepository surveyRepository,
        ISurveyUnitOfWork unitOfWork,
        ILogger<AutoSaveAnswersCommandHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<ApplicationResult<AutoSaveAnswersResponse>> Handle(AutoSaveAnswersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Efficiently load survey with response and questions
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
            {
                return ApplicationResult<AutoSaveAnswersResponse>.Failure("نظرسنجی یافت نشد");
            }

            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
            {
                return ApplicationResult<AutoSaveAnswersResponse>.Failure("پاسخ یافت نشد");
            }

            // Check if response is submitted
            if (response.SubmittedAt.HasValue)
            {
                return ApplicationResult<AutoSaveAnswersResponse>.Failure("RESPONSE_ALREADY_SUBMITTED: پاسخ قبلاً ارسال شده است");
            }

            var savedCount = 0;
            var invalids = new List<InvalidAnswerDto>();

            // Ensure response exists in database first
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var answerDto in request.Answers)
            {
                try
                {
                    // Get question
                    var question = survey.Questions.FirstOrDefault(q => q.Id == answerDto.QuestionId);
                    if (question == null)
                    {
                        invalids.Add(new InvalidAnswerDto
                        {
                            QuestionId = answerDto.QuestionId,
                            ErrorMessage = "سوال یافت نشد"
                        });
                        continue;
                    }

                    // Validate answer
                    var validationResult = ValidateAnswer(question, answerDto.TextAnswer, answerDto.SelectedOptionIds);
                    if (!validationResult.IsValid)
                    {
                        invalids.Add(new InvalidAnswerDto
                        {
                            QuestionId = answerDto.QuestionId,
                            ErrorMessage = validationResult.ErrorMessage
                        });
                        continue;
                    }

                    // Use domain aggregate to set the answer
                    survey.SetResponseAnswer(request.ResponseId, answerDto.QuestionId, answerDto.TextAnswer, answerDto.SelectedOptionIds);

                    savedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving answer for question {QuestionId}", answerDto.QuestionId);
                    invalids.Add(new InvalidAnswerDto
                    {
                        QuestionId = answerDto.QuestionId,
                        ErrorMessage = "خطا در ذخیره پاسخ"
                    });
                }
            }

            // Save all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseDto = new AutoSaveAnswersResponse
            {
                SavedCount = savedCount,
                Invalids = invalids
            };

            return ApplicationResult<AutoSaveAnswersResponse>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error autosaving answers for response {ResponseId}", request.ResponseId);
            return ApplicationResult<AutoSaveAnswersResponse>.Failure("خطا در ذخیره خودکار پاسخ‌ها");
        }
    }

    private static (bool IsValid, string ErrorMessage) ValidateAnswer(
        Question question, 
        string? textAnswer, 
        List<Guid>? selectedOptionIds)
    {
        // Check if question is required and has no answer
        if (question.IsRequired)
        {
            var hasTextAnswer = !string.IsNullOrWhiteSpace(textAnswer);
            var hasSelectedOptions = selectedOptionIds != null && selectedOptionIds.Any();

            if (!hasTextAnswer && !hasSelectedOptions)
            {
                return (false, "این سوال اجباری است");
            }
        }

        // Validate based on question type
        switch (question.Kind)
        {
            case QuestionKind.FixedMCQ4:
            case QuestionKind.ChoiceSingle:
                if (selectedOptionIds != null && selectedOptionIds.Count > 1)
                {
                    return (false, "فقط یک گزینه می‌توانید انتخاب کنید");
                }
                break;

            case QuestionKind.ChoiceMulti:
                // Multi choice allows multiple selections
                break;

            case QuestionKind.Textual:
                // Textual questions only need text answer
                break;
        }

        return (true, string.Empty);
    }
}
