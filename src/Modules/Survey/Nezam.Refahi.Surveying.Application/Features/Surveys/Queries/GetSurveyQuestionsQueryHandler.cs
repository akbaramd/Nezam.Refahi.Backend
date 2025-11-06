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

namespace Nezam.Refahi.Surveying.Application.Features.Surveys.Queries;

/// <summary>
/// Handler for GetSurveyQuestionsQuery
/// </summary>
public class GetSurveyQuestionsQueryHandler : IRequestHandler<GetSurveyQuestionsQuery, ApplicationResult<SurveyQuestionsResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetSurveyQuestionsQueryHandler> _logger;
    private readonly IMemberService _memberService;

    public GetSurveyQuestionsQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetSurveyQuestionsQueryHandler> logger,
        IMemberService memberService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberService = memberService;
    }

    public async Task<ApplicationResult<SurveyQuestionsResponse>> Handle(GetSurveyQuestionsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var survey = await _surveyRepository.GetByIdAsync(request.SurveyId, cancellationToken);
            if (survey == null)
                return ApplicationResult<SurveyQuestionsResponse>.Failure("نظرسنجی یافت نشد");

            // Get member info if national number is provided
            Guid? memberId = null;
            if (!string.IsNullOrWhiteSpace(request.UserNationalNumber))
            {
                var nationalId = new NationalId(request.UserNationalNumber);
                var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
                memberId = memberDetail?.Id;
            }

            var questions = survey.Questions
                .OrderBy(q => q.Order)
                .Select(q => MapQuestionToDto(q, request.IncludeUserAnswers, memberId))
                .ToList();

            // Include user answers if requested
            if (request.IncludeUserAnswers && memberId.HasValue)
            {
                var userResponse = survey.Responses
                    .Where(r => r.Participant.MemberId == memberId.Value)
                    .OrderByDescending(r => r.AttemptNumber)
                    .FirstOrDefault();

                foreach (var questionDto in questions)
                {
                    var userAnswer = userResponse?.GetQuestionAnswer(questionDto.Id, 1);
                    if (userAnswer != null)
                    {
                        questionDto.UserAnswer = MapQuestionAnswerToDto(userAnswer);
                        questionDto.IsAnswered = true;
                        questionDto.IsComplete = IsQuestionAnswerComplete(questionDto, userAnswer);
                    }
                }
            }

            var response = new SurveyQuestionsResponse
            {
                SurveyId = survey.Id,
                SurveyTitle = survey.Title,
                SurveyDescription = string.Empty, // Survey entity doesn't have Description property
                Questions = questions,
                HasUserResponse = !string.IsNullOrWhiteSpace(request.UserNationalNumber) && questions.Any(q => q.IsAnswered),
                UserResponseId = null, // This would need to be fetched from response repository
                UserAttemptNumber = null // This would need to be fetched from response repository
            };

            return ApplicationResult<SurveyQuestionsResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting survey questions for SurveyId {SurveyId}", request.SurveyId);
            return ApplicationResult<SurveyQuestionsResponse>.Failure(ex, "خطا در دریافت سوالات نظرسنجی");
        }
    }

    private static QuestionDto MapQuestionToDto(Question question, bool includeUserAnswers, Guid? memberId)
    {
        var questionDto = new QuestionDto
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

        return questionDto;
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

    private static QuestionAnswerDto MapQuestionAnswerToDto(QuestionAnswer questionAnswer)
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

    private static QuestionAnswerOptionDto MapAnswerOptionToDto(QuestionAnswerOption answerOption)
    {
        return new QuestionAnswerOptionDto
        {
            Id = answerOption.Id,
            QuestionAnswerId = answerOption.QuestionAnswerId,
            OptionId = answerOption.OptionId,
            OptionText = answerOption.OptionText
        };
    }

    private static bool IsQuestionAnswerComplete(QuestionDto question, QuestionAnswer answer)
    {
        if (question.IsRequired && !answer.HasAnswer())
            return false;

        // Check if answer matches question type
        switch (question.Kind)
        {
            case "FixedMCQ4":
            case "ChoiceSingle":
                return answer.SelectedOptions.Count == 1;
            case "ChoiceMulti":
                return answer.SelectedOptions.Count > 0;
            case "Textual":
                return !string.IsNullOrEmpty(answer.TextAnswer);
            default:
                return false;
        }
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
