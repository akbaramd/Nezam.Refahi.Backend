using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Queries;

/// <summary>
/// Handler for GetSurveyQuestionsWithAnswersQuery
/// </summary>
public class GetSurveyQuestionsWithAnswersQueryHandler : IRequestHandler<GetSurveyQuestionsWithAnswersQuery, ApplicationResult<SurveyQuestionsWithAnswersResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetSurveyQuestionsWithAnswersQueryHandler> _logger;
    private readonly IMemberInfoService _memberInfoService;

    public GetSurveyQuestionsWithAnswersQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetSurveyQuestionsWithAnswersQueryHandler> logger,
        IMemberInfoService memberInfoService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberInfoService = memberInfoService;
    }

    public async Task<ApplicationResult<SurveyQuestionsWithAnswersResponse>> Handle(GetSurveyQuestionsWithAnswersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get survey with questions
            var survey = await _surveyRepository.GetWithQuestionsAsync(request.SurveyId, cancellationToken);
            if (survey == null)
                return ApplicationResult<SurveyQuestionsWithAnswersResponse>.Failure("نظرسنجی یافت نشد");

            var response = new SurveyQuestionsWithAnswersResponse
            {
                SurveyId = survey.Id,
                SurveyTitle = survey.Title,
                SurveyDescription = string.Empty, // Survey entity doesn't have Description property
                Questions = new List<QuestionWithAnswersDto>(),
                HasUserResponse = false,
                TotalQuestions = survey.Questions.Count,
                AnsweredQuestions = 0,
                CompletionPercentage = 0
            };

            // Get member info if national number is provided
            Guid? memberId = null;
            if (!string.IsNullOrWhiteSpace(request.UserNationalNumber))
            {
                var nationalId = new NationalId(request.UserNationalNumber);
                var memberInfo = await _memberInfoService.GetMemberInfoAsync(nationalId);
                memberId = memberInfo?.Id;
            }

            // Get survey with responses if needed
            Survey? surveyWithResponses = null;
            if (request.ResponseId.HasValue || memberId.HasValue)
            {
                surveyWithResponses = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            }

            // Determine which response(s) to get answers for
            List<Response> responses = new();
            Response? specificResponse = null;

            if (request.ResponseId.HasValue)
            {
                // Get specific response from survey aggregate
                specificResponse = surveyWithResponses?.Responses.FirstOrDefault(r => r.Id == request.ResponseId.Value);
                if (specificResponse != null)
                {
                    responses.Add(specificResponse);
                    response.ResponseInfo = MapResponseToDto(specificResponse, survey.Questions.Count);
                }
            }
            else if (memberId.HasValue)
            {
                if (request.AttemptNumber.HasValue)
                {
                    // Get specific attempt
                    var memberResponses = surveyWithResponses?.Responses
                        .Where(r => r.Participant.MemberId == memberId.Value)
                        .ToList() ?? new List<Response>();
                    specificResponse = memberResponses.FirstOrDefault(r => r.AttemptNumber == request.AttemptNumber.Value); 
                    if (specificResponse != null)
                    {
                        responses.Add(specificResponse);
                        response.ResponseInfo = MapResponseToDto(specificResponse, survey.Questions.Count);
                    }
                }
                else if (request.IncludeAllAttempts)
                {
                    // Get all attempts for this member
                    var memberResponses = surveyWithResponses?.Responses
                        .Where(r => r.Participant.MemberId == memberId.Value)
                        .OrderByDescending(r => r.AttemptNumber)
                        .ToList() ?? new List<Response>();
                    responses.AddRange(memberResponses);
                    if (memberResponses.Any())
                    {
                        response.ResponseInfo = MapResponseToDto(memberResponses.First(), survey.Questions.Count);
                    }
                }
                else
                {
                    // Get latest attempt
                    specificResponse = surveyWithResponses?.Responses
                        .Where(r => r.Participant.MemberId == memberId.Value)
                        .OrderByDescending(r => r.AttemptNumber)
                        .FirstOrDefault();
                    if (specificResponse != null)
                    {
                        responses.Add(specificResponse);
                        response.ResponseInfo = MapResponseToDto(specificResponse, survey.Questions.Count);
                    }
                }
            }

            // Get all question answers for the responses
            var allQuestionAnswers = new List<Domain.Entities.QuestionAnswer>();
            foreach (var resp in responses)
            {
                var questionAnswers = resp.QuestionAnswers.ToList();
                allQuestionAnswers.AddRange(questionAnswers);
            }

            // Map questions with answers
            var questionsWithAnswers = new List<QuestionWithAnswersDto>();
            foreach (var question in survey.Questions.OrderBy(q => q.Order))
            {
                var questionDto = MapQuestionToDto(question);
                
                // Get all answers for this question
                var questionAnswers = allQuestionAnswers.Where(qa => qa.QuestionId == question.Id).ToList();
                questionDto.UserAnswers = questionAnswers.Select(MapQuestionAnswerToDto).ToList();
                questionDto.AnswerCount = questionAnswers.Count;
                
                // Set latest answer (QuestionAnswer is now Entity, no CreatedAt - use first available)
                questionDto.LatestAnswer = questionAnswers.FirstOrDefault() != null 
                    ? MapQuestionAnswerToDto(questionAnswers.First())
                    : null;
                
                // Check if answered
                questionDto.IsAnswered = questionAnswers.Any(qa => qa.HasAnswer());
                questionDto.IsComplete = questionDto.IsAnswered && (!question.IsRequired || questionDto.LatestAnswer != null);
                
                questionsWithAnswers.Add(questionDto);
            }

            response.Questions = questionsWithAnswers;
            response.HasUserResponse = responses.Any();
            response.AnsweredQuestions = questionsWithAnswers.Count(q => q.IsAnswered);
            response.CompletionPercentage = response.TotalQuestions > 0 
                ? (decimal)response.AnsweredQuestions / response.TotalQuestions * 100 
                : 0;

            return ApplicationResult<SurveyQuestionsWithAnswersResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting survey questions with answers for SurveyId {SurveyId}", request.SurveyId);
            return ApplicationResult<SurveyQuestionsWithAnswersResponse>.Failure("خطا در دریافت سوالات نظرسنجی با پاسخ‌ها");
        }
    }

    private static QuestionWithAnswersDto MapQuestionToDto(Question question)
    {
        return new QuestionWithAnswersDto
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

    private static Contracts.Dtos.QuestionOptionDto MapOptionToDto(QuestionOption option)
    {
        return new Contracts.Dtos.QuestionOptionDto
        {
            Id = option.Id,
            QuestionId = option.QuestionId,
            Text = option.Text,
            Order = option.Order,
            IsActive = option.IsActive
        };
    }

    private static QuestionAnswerDto MapQuestionAnswerToDto(Domain.Entities.QuestionAnswer questionAnswer)
    {
        return new QuestionAnswerDto
        {
            Id = questionAnswer.Id,
            ResponseId = questionAnswer.ResponseId,
            QuestionId = questionAnswer.QuestionId,
            TextAnswer = questionAnswer.TextAnswer,
            SelectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToList()
        };
    }

    private static ResponseInfoDto MapResponseToDto(Response response, int totalQuestions)
    {
        return new ResponseInfoDto
        {
            ResponseId = response.Id,
            SurveyId = response.SurveyId,
            AttemptNumber = response.AttemptNumber,
            CreatedAt = response.SubmittedAt?.DateTime ?? DateTime.MinValue,
            ParticipantDisplayName = response.Participant.GetDisplayName(),
            ParticipantShortIdentifier = response.Participant.GetShortIdentifier(),
            IsAnonymous = response.Participant.IsAnonymous
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
