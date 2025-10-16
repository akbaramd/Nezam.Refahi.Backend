using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Application.Queries;

/// <summary>
/// Handler for GetNextQuestionQuery
/// </summary>
public class GetNextQuestionQueryHandler : IRequestHandler<GetNextQuestionQuery, ApplicationResult<NextQuestionResponseDto>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetNextQuestionQueryHandler> _logger;

    public GetNextQuestionQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetNextQuestionQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<NextQuestionResponseDto>> Handle(GetNextQuestionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with response and questions
            var survey = await _surveyRepository.GetSurveyWithResponseAndQuestionsAsync(request.ResponseId, cancellationToken);
            if (survey == null)
                return ApplicationResult<NextQuestionResponseDto>.Failure("نظرسنجی یا پاسخ یافت نشد");

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
                return ApplicationResult<NextQuestionResponseDto>.Failure("پاسخ یافت نشد");

            // Authorization check if MemberId is provided
            if (request.MemberId.HasValue && response.Participant.MemberId != request.MemberId.Value)
                return ApplicationResult<NextQuestionResponseDto>.Failure("شما دسترسی به این پاسخ ندارید");

            // Get all questions ordered by Order
            var orderedQuestions = survey.Questions.OrderBy(q => q.Order).ToList();
            var totalQuestions = orderedQuestions.Count;

            // Get all question answers for this response
            var questionAnswers = response.QuestionAnswers.ToList();
            var answeredQuestions = questionAnswers.Count(qa => qa.HasAnswer());

            // Determine next question
            Question? nextQuestion = null;
            Question? currentQuestion = null;
            bool isFirstQuestion = false;
            bool isLastQuestion = false;
            int currentQuestionOrder = 0;
            int nextQuestionOrder = 0;

            if (request.CurrentQuestionId == null)
            {
                // Return first question
                nextQuestion = orderedQuestions.FirstOrDefault();
                isFirstQuestion = true;
                nextQuestionOrder = 1;
            }
            else
            {
                // Find current question
                currentQuestion = orderedQuestions.FirstOrDefault(q => q.Id == request.CurrentQuestionId);
                if (currentQuestion == null)
                    return ApplicationResult<NextQuestionResponseDto>.Failure("سوال فعلی یافت نشد");

                currentQuestionOrder = currentQuestion.Order;
                
                // Find next question
                nextQuestion = orderedQuestions.FirstOrDefault(q => q.Order > currentQuestion.Order);
                
                if (nextQuestion == null)
                {
                    // No next question - this is the last question
                    isLastQuestion = true;
                }
                else
                {
                    nextQuestionOrder = nextQuestion.Order;
                }
            }

            // Create response
            var responseDto = new NextQuestionResponseDto
            {
                SurveyId = request.SurveyId,
                ResponseId = request.ResponseId,
                CurrentQuestionId = request.CurrentQuestionId,
                NextQuestionId = nextQuestion?.Id,
                HasNextQuestion = nextQuestion != null,
                IsFirstQuestion = isFirstQuestion,
                IsLastQuestion = isLastQuestion,
                CurrentQuestionOrder = currentQuestionOrder,
                NextQuestionOrder = nextQuestionOrder,
                TotalQuestions = totalQuestions,
                AnsweredQuestions = answeredQuestions,
                ProgressPercentage = totalQuestions > 0 ? (decimal)answeredQuestions / totalQuestions * 100 : 0,
                NextQuestion = nextQuestion != null ? MapQuestionToDto(nextQuestion) : null,
                UserAnswer = null,
                Navigation = new QuestionNavigationDto(),
                Progress = new SurveyProgressDto()
            };

            // Get user's answer for next question if requested and exists
            if (request.IncludeUserAnswer && nextQuestion != null)
            {
                var userAnswer = questionAnswers.FirstOrDefault(qa => qa.QuestionId == nextQuestion.Id);
                if (userAnswer != null)
                {
                    responseDto.UserAnswer = MapQuestionAnswerToDto(userAnswer, nextQuestion);
                }
            }

            // Calculate navigation information
            responseDto.Navigation = CalculateNavigation(orderedQuestions, currentQuestion, nextQuestion);

            // Calculate progress information
            responseDto.Progress = CalculateProgress(answeredQuestions, totalQuestions, nextQuestionOrder, isLastQuestion);

            return ApplicationResult<NextQuestionResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next question for SurveyId {SurveyId}, ResponseId {ResponseId}", 
                request.SurveyId, request.ResponseId);
            return ApplicationResult<NextQuestionResponseDto>.Failure("خطا در دریافت سوال بعدی");
        }
    }

    private static QuestionDetailsDto MapQuestionToDto(Question question)
    {
        return new QuestionDetailsDto
        {
            Id = question.Id,
            SurveyId = question.SurveyId,
            Kind = question.Kind.ToString(),
            KindText = GetQuestionKindText(question.Kind),
            Text = question.Text,
            Order = question.Order,
            IsRequired = question.IsRequired,
            Options = question.Options
                .Where(o => o.IsActive)
                .OrderBy(o => o.Order)
                .Select(MapOptionToDto)
                .ToList()
        };
    }

    private static Contracts.Dtos.QuestionOptionDto MapOptionToDto(QuestionOption? option)
    {
        if (option == null)
            throw new ArgumentNullException(nameof(option));
            
        return new Contracts.Dtos.QuestionOptionDto
        {
            Id = option.Id,
            QuestionId = option.QuestionId,
            Text = option.Text,
            Order = option.Order,
            IsActive = option.IsActive
        };
    }

    private static QuestionAnswerDetailsDto MapQuestionAnswerToDto(Domain.Entities.QuestionAnswer questionAnswer, Question question)
    {
        return new QuestionAnswerDetailsDto
        {
            Id = questionAnswer.Id,
            ResponseId = questionAnswer.ResponseId,
            QuestionId = questionAnswer.QuestionId,
            TextAnswer = questionAnswer.TextAnswer,
            SelectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToList(),
            SelectedOptions = questionAnswer.SelectedOptions
                .Select(so => question.Options.FirstOrDefault(o => o.Id == so.OptionId))
                .Where(o => o != null)
                .Select(MapOptionToDto)
                .ToList(),
            IsAnswered = questionAnswer.HasAnswer(),
            IsComplete = questionAnswer.HasAnswer()
        };
    }

    private static QuestionNavigationDto CalculateNavigation(List<Question> orderedQuestions, Question? currentQuestion, Question? nextQuestion)
    {
        var navigation = new QuestionNavigationDto();

        if (currentQuestion != null)
        {
            // Find previous question
            var previousQuestion = orderedQuestions.LastOrDefault(q => q.Order < currentQuestion.Order);
            navigation.PreviousQuestionId = previousQuestion?.Id;
            navigation.CanGoBack = previousQuestion != null;
        }

        navigation.NextQuestionId = nextQuestion?.Id;
        navigation.CanGoForward = nextQuestion != null;
        navigation.CanSkip = nextQuestion?.IsRequired == false;
        navigation.IsRequired = nextQuestion?.IsRequired ?? false;

        return navigation;
    }

    private static SurveyProgressDto CalculateProgress(int answeredQuestions, int totalQuestions, int nextQuestionOrder, bool isLastQuestion)
    {
        var currentStep = isLastQuestion ? totalQuestions : nextQuestionOrder;
        var remainingQuestions = totalQuestions - answeredQuestions;
        var completionPercentage = totalQuestions > 0 ? (decimal)answeredQuestions / totalQuestions * 100 : 0;
        var isComplete = answeredQuestions == totalQuestions;

        var progressText = isComplete ? "نظرسنجی تکمیل شده است" :
                           remainingQuestions == 0 ? "تمام سوالات پاسخ داده شده" :
                           $"سوال {currentStep} از {totalQuestions}";

        return new SurveyProgressDto
        {
            CurrentStep = currentStep,
            TotalSteps = totalQuestions,
            CompletionPercentage = completionPercentage,
            RemainingQuestions = remainingQuestions,
            IsComplete = isComplete,
            ProgressText = progressText
        };
    }

    private static string GetQuestionKindText(QuestionKind kind)
    {
        return kind switch
        {
            QuestionKind.FixedMCQ4 => "چهار گزینه‌ای ثابت",
            QuestionKind.ChoiceSingle => "انتخاب تک",
            QuestionKind.ChoiceMulti => "انتخاب چندگانه",
            QuestionKind.Textual => "متنی",
            _ => "نامشخص"
        };
    }
}
