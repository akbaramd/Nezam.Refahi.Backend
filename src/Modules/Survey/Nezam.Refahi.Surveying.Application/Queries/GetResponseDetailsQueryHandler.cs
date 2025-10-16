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
/// Handler for GetResponseDetailsQuery
/// </summary>
public class GetResponseDetailsQueryHandler : IRequestHandler<GetResponseDetailsQuery, ApplicationResult<ResponseDetailsDto>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetResponseDetailsQueryHandler> _logger;

    public GetResponseDetailsQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetResponseDetailsQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<ResponseDetailsDto>> Handle(GetResponseDetailsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with response and all data
            var survey = await _surveyRepository.GetSurveyWithResponseAndAllDataAsync(request.ResponseId, cancellationToken);
            if (survey == null)
                return ApplicationResult<ResponseDetailsDto>.Failure("نظرسنجی یا پاسخ یافت نشد");

            // Get response from survey aggregate
            var response = survey.Responses.FirstOrDefault(r => r.Id == request.ResponseId);
            if (response == null)
                return ApplicationResult<ResponseDetailsDto>.Failure("پاسخ یافت نشد");

            // Authorization check if MemberId is provided
            if (request.MemberId.HasValue && response.Participant.MemberId != request.MemberId.Value)
                return ApplicationResult<ResponseDetailsDto>.Failure("شما دسترسی به این پاسخ ندارید");
            // Map to DTO
            var responseDto = new ResponseDetailsDto
            {
                Id = response.Id,
                SurveyId = response.SurveyId,
                AttemptNumber = response.AttemptNumber,
                Participant = MapParticipantToDto(response.Participant),
                Survey = request.IncludeSurveyDetails ? MapSurveyToDto(survey) : null,
                QuestionAnswers = new List<QuestionAnswerDetailsDto>(),
                Statistics = new ResponseStatisticsDto(),
                Status = new ResponseStatusDto()
            };

            // Map question answers from response aggregate
            var questionAnswerDtos = new List<QuestionAnswerDetailsDto>();
            foreach (var questionAnswer in response.QuestionAnswers)
            {
                var questionAnswerDto = MapQuestionAnswerToDto(questionAnswer);
                
                // Include question details if requested
                if (request.IncludeQuestionDetails && survey != null)
                {
                    var question = survey.Questions.FirstOrDefault(q => q.Id == questionAnswer.QuestionId);
                    if (question != null)
                    {
                        questionAnswerDto.Question = MapQuestionToDto(question);
                        questionAnswerDto.SelectedOptions = questionAnswer.SelectedOptions
                            .Select(so => question.Options.FirstOrDefault(o => o.Id == so.OptionId))
                            .Where(o => o != null)
                            .Select(MapOptionToDto)
                            .ToList();
                    }
                }
                
                questionAnswerDtos.Add(questionAnswerDto);
            }

            responseDto.QuestionAnswers = questionAnswerDtos.OrderBy(qa => qa.Question?.Order ?? 0).ToList();

            // Calculate statistics
            responseDto.Statistics = CalculateResponseStatistics(response, questionAnswerDtos, survey);

            // Calculate status
            responseDto.Status = CalculateResponseStatus(response, questionAnswerDtos, survey);

            return ApplicationResult<ResponseDetailsDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting response details for ResponseId {ResponseId}", request.ResponseId);
            return ApplicationResult<ResponseDetailsDto>.Failure("خطا در دریافت جزئیات پاسخ");
        }
    }

    private static ParticipantInfoDto MapParticipantToDto(Domain.ValueObjects.ParticipantInfo participant)
    {
        return new ParticipantInfoDto
        {
            MemberId = participant.MemberId,
            ParticipantHash = participant.ParticipantHash,
            IsAnonymous = participant.IsAnonymous,
            DemographyData = null // Participant doesn't have DemographySnapshot
        };
    }

    private static SurveyBasicInfoDto MapSurveyToDto(Survey survey)
    {
        return new SurveyBasicInfoDto
        {
            Id = survey.Id,
            Title = survey.Title,
            State = survey.State.ToString(),
            StateText = GetSurveyStateText(survey.State),
            IsActive = survey.State == SurveyState.Active,
            IsAnonymous = survey.IsAnonymous,
            StartAt = survey.StartAt?.DateTime,
            EndAt = survey.EndAt?.DateTime,
            TotalQuestions = survey.Questions.Count,
            RequiredQuestions = survey.Questions.Count(q => q.IsRequired)
        };
    }

    private static QuestionAnswerDetailsDto MapQuestionAnswerToDto(Domain.Entities.QuestionAnswer questionAnswer)
    {
        return new QuestionAnswerDetailsDto
        {
            Id = questionAnswer.Id,
            ResponseId = questionAnswer.ResponseId,
            QuestionId = questionAnswer.QuestionId,
            TextAnswer = questionAnswer.TextAnswer,
            SelectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToList(),
            IsAnswered = questionAnswer.HasAnswer(),
            IsComplete = questionAnswer.HasAnswer()
        };
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

    private static ResponseStatisticsDto CalculateResponseStatistics(Response response, IEnumerable<QuestionAnswerDetailsDto> questionAnswers, Survey? survey)
    {
        var answeredQuestions = questionAnswers.Count(qa => qa.IsAnswered);
        var totalQuestions = survey?.Questions.Count ?? 0;
        var requiredQuestions = survey?.Questions.Count(q => q.IsRequired) ?? 0;
        var requiredAnsweredQuestions = questionAnswers.Count(qa => 
            qa.IsAnswered && survey?.Questions.FirstOrDefault(q => q.Id == qa.QuestionId)?.IsRequired == true);

        var completionPercentage = totalQuestions > 0 ? (decimal)answeredQuestions / totalQuestions * 100 : 0;
        var isComplete = answeredQuestions == totalQuestions && requiredAnsweredQuestions == requiredQuestions;

        var firstAnswerAt = (DateTime?)null; // QuestionAnswer is now Entity, no CreatedAt
        var lastAnswerAt = (DateTime?)null; // QuestionAnswer is now Entity, no CreatedAt
        var timeSpent = firstAnswerAt.HasValue && lastAnswerAt.HasValue ? lastAnswerAt.Value - firstAnswerAt.Value : (TimeSpan?)null;

        return new ResponseStatisticsDto
        {
            TotalQuestions = totalQuestions,
            AnsweredQuestions = answeredQuestions,
            RequiredQuestions = requiredQuestions,
            RequiredAnsweredQuestions = requiredAnsweredQuestions,
            CompletionPercentage = completionPercentage,
            IsComplete = isComplete,
            TimeSpent = timeSpent,
            FirstAnswerAt = firstAnswerAt,
            LastAnswerAt = lastAnswerAt
        };
    }

    private static ResponseStatusDto CalculateResponseStatus(Response response, IEnumerable<QuestionAnswerDetailsDto> questionAnswers, Survey? survey)
    {
        var answeredQuestions = questionAnswers.Count(qa => qa.IsAnswered);
        var totalQuestions = survey?.Questions.Count ?? 0;
        var isComplete = answeredQuestions == totalQuestions;

        var canContinue = survey != null && survey.State == SurveyState.Active;
        var canSubmit = isComplete && canContinue;
        var isSubmitted = false; // This would need to be tracked in the domain

        var statusMessage = isComplete ? "نظرسنجی تکمیل شده است" : 
                           answeredQuestions == 0 ? "هنوز شروع نشده است" : 
                           $"در حال تکمیل ({answeredQuestions}/{totalQuestions})";

        return new ResponseStatusDto
        {
            CanContinue = canContinue,
            CanSubmit = canSubmit,
            IsSubmitted = isSubmitted,
            StatusMessage = statusMessage,
            ValidationErrors = new List<string>()
        };
    }

    private static string GetSurveyStateText(SurveyState state)
    {
        return state switch
        {
            SurveyState.Draft => "پیش‌نویس",
            SurveyState.Scheduled => "زمان‌بندی شده",
            SurveyState.Active => "فعال",
            SurveyState.Closed => "بسته شده",
            SurveyState.Archived => "آرشیو شده",
            _ => "نامشخص"
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
