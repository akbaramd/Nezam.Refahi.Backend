using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Surveying.Contracts.Commands;
using Nezam.Refahi.Surveying.Contracts.Dtos;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Surveying.Application.Commands;

/// <summary>
/// Handler for GetUserResponseQuery
/// </summary>
public class GetUserResponseQueryHandler : IRequestHandler<GetUserResponseQuery, ApplicationResult<ResponseDto>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetUserResponseQueryHandler> _logger;

    public GetUserResponseQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetUserResponseQueryHandler> logger)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
    }

    public async Task<ApplicationResult<ResponseDto>> Handle(GetUserResponseQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Load survey with responses and questions
            var survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            if (survey == null)
                return ApplicationResult<ResponseDto>.Failure("نظرسنجی یافت نشد");

            Response? response;

            if (request.AttemptNumber.HasValue)
            {
                // Get specific attempt
                response = survey.Responses.FirstOrDefault(r => 
                    r.Participant.MemberId == request.UserId && 
                    r.AttemptNumber == request.AttemptNumber.Value);
            }
            else
            {
                // Get latest attempt
                response = survey.Responses
                    .Where(r => r.Participant.MemberId == request.UserId)
                    .OrderByDescending(r => r.AttemptNumber)
                    .FirstOrDefault();
            }

            if (response == null)
                return ApplicationResult<ResponseDto>.Failure("پاسخ یافت نشد");

            // Map to DTO
            var responseDto = new ResponseDto
            {
                Id = response.Id,
                SurveyId = response.SurveyId,
                AttemptNumber = response.AttemptNumber,
                ParticipantDisplayName = response.Participant.GetDisplayName(),
                ParticipantShortIdentifier = response.Participant.GetShortIdentifier(),
                IsAnonymous = response.Participant.IsAnonymous,
                QuestionAnswers = response.QuestionAnswers.Select(MapQuestionAnswerToDto).ToList(),
                TotalQuestions = survey.Questions.Count,
                AnsweredQuestions = response.QuestionAnswers.Count(qa => qa.HasAnswer()),
                RequiredQuestions = survey.Questions.Count(q => q.IsRequired),
                RequiredAnsweredQuestions = survey.Questions
                    .Where(q => q.IsRequired)
                    .Count(q => response.QuestionAnswers.Any(qa => qa.QuestionId == q.Id && qa.HasAnswer())),
                IsComplete = response.QuestionAnswers.Count(qa => qa.HasAnswer()) == survey.Questions.Count,
                CompletionPercentage = survey.Questions.Count > 0 ? 
                    (decimal)response.QuestionAnswers.Count(qa => qa.HasAnswer()) / survey.Questions.Count * 100 : 0
            };

            return ApplicationResult<ResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user response for SurveyId {SurveyId}, UserId {UserId}", 
                request.SurveyId, request.UserId);
            return ApplicationResult<ResponseDto>.Failure("خطا در دریافت پاسخ کاربر");
        }
    }

    private static QuestionAnswerDto MapQuestionAnswerToDto(Domain.Entities.QuestionAnswer questionAnswer)
    {
        return new QuestionAnswerDto
        {
            Id = questionAnswer.Id,
            ResponseId = questionAnswer.ResponseId,
            QuestionId = questionAnswer.QuestionId,
            TextAnswer = questionAnswer.TextAnswer,
            SelectedOptionIds = questionAnswer.SelectedOptions.Select(so => so.OptionId).ToList(),
            SelectedOptions = questionAnswer.SelectedOptions.Select(MapAnswerOptionToDto).ToList(),
            HasAnswer = !string.IsNullOrEmpty(questionAnswer.TextAnswer) || questionAnswer.SelectedOptions.Any()
        };
    }

    private static QuestionAnswerOptionDto MapAnswerOptionToDto(Domain.Entities.QuestionAnswerOption answerOption)
    {
        return new QuestionAnswerOptionDto
        {
            Id = answerOption.Id,
            QuestionAnswerId = answerOption.QuestionAnswerId,
            OptionId = answerOption.OptionId,
            OptionText = answerOption.OptionText
        };
    }
}
