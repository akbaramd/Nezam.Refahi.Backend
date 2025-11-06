using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Membership.Contracts.Services;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Application.Services;
using Nezam.Refahi.Surveying.Contracts.Queries;
using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.Repositories;

namespace Nezam.Refahi.Surveying.Application.Features.Responses.Queries;

/// <summary>
/// Handler for GetParticipationStatusQuery
/// </summary>
public class GetParticipationStatusQueryHandler : IRequestHandler<GetParticipationStatusQuery, ApplicationResult<ParticipationStatusResponse>>
{
    private readonly ISurveyRepository _surveyRepository;
    private readonly ILogger<GetParticipationStatusQueryHandler> _logger;
    private readonly IMemberService _memberService;

    public GetParticipationStatusQueryHandler(
        ISurveyRepository surveyRepository,
        ILogger<GetParticipationStatusQueryHandler> logger,
        IMemberService memberDetailService)
    {
        _surveyRepository = surveyRepository;
        _logger = logger;
        _memberService = memberDetailService;
    }

    public async Task<ApplicationResult<ParticipationStatusResponse>> Handle(
        GetParticipationStatusQuery request, 
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting participation status for user {UserNationalNumber} in survey {SurveyId}", 
                request.UserNationalNumber, request.SurveyId);

            // Validate user national number
            if (string.IsNullOrWhiteSpace(request.UserNationalNumber))
            {
                _logger.LogWarning("User national number is null or empty");
                return ApplicationResult<ParticipationStatusResponse>.Failure("User national number is required");
            }

            // Get member info from national number
            var nationalId = new NationalId(request.UserNationalNumber);
            var memberDetail = await _memberService.GetMemberDetailByNationalCodeAsync(nationalId);
            
            if (memberDetail == null)
            {
                _logger.LogWarning("Member with national number {UserNationalNumber} not found", request.UserNationalNumber);
                return ApplicationResult<ParticipationStatusResponse>.Failure("Member not found");
            }

            // Get survey with responses
            var survey = await _surveyRepository.GetWithResponsesAsync(request.SurveyId, cancellationToken);
            if (survey == null)
            {
                _logger.LogWarning("Survey {SurveyId} not found", request.SurveyId);
                return ApplicationResult<ParticipationStatusResponse>.Failure("Survey not found");
            }

            // Get member's responses from survey aggregate
            var memberResponses = survey.Responses
                .Where(r => r.Participant.MemberId == memberDetail.Id)
                .OrderByDescending(r => r.AttemptNumber)
                .ToList();

            var response = await BuildParticipationStatus(survey, request.UserNationalNumber, memberDetail.Id, memberResponses.FirstOrDefault());
            
            _logger.LogInformation("Successfully retrieved participation status for user {UserNationalNumber} in survey {SurveyId}", 
                request.UserNationalNumber, request.SurveyId);
            return ApplicationResult<ParticipationStatusResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting participation status for user {UserNationalNumber} in survey {SurveyId}", 
                request.UserNationalNumber, request.SurveyId);
            return ApplicationResult<ParticipationStatusResponse>.Failure("An error occurred while retrieving participation status");
        }
    }

    private async Task<ParticipationStatusResponse> BuildParticipationStatus(
        Survey survey, 
        string userNationalNumber,
        Guid memberId, 
        Response? memberResponse)
    {
        var response = new ParticipationStatusResponse
        {
            SurveyId = survey.Id,
            UserNationalNumber = userNationalNumber,
            MemberId = memberId,
            MaxAllowedAttempts = survey.ParticipationPolicy.MaxAttemptsPerMember,
            AllowMultipleSubmissions = survey.ParticipationPolicy.AllowMultipleSubmissions
        };

        // Check eligibility
        var eligibility = CheckEligibility(survey, memberResponse);
        response.IsEligible = eligibility.IsEligible;
        response.EligibilityReason = eligibility.Reason;

        if (memberResponse != null)
        {
            response.TotalAttempts = memberResponse.AttemptNumber;
            response.CurrentResponseId = memberResponse.Id;
            response.CurrentAttemptStatus = GetAttemptStatus(memberResponse);
            response.CurrentAttemptStartedAt = memberResponse.SubmittedAt?.DateTime;
            response.CurrentAttemptSubmittedAt = memberResponse.SubmittedAt?.DateTime;
            
            // Debug logging
            _logger.LogInformation("Response {ResponseId} - SubmittedAt: {SubmittedAt}, Status: {Status}", 
                memberResponse.Id, memberResponse.SubmittedAt, response.CurrentAttemptStatus);

            // Check if can start new attempt
            response.CanStartNewAttempt = CanStartNewAttempt(survey, memberResponse);

            // Check cooldown
            var cooldownInfo = CheckCooldown(survey, memberResponse);
            response.IsInCooldown = cooldownInfo.IsInCooldown;
            response.CooldownEndsAt = cooldownInfo.EndsAt;
            response.RemainingCooldown = cooldownInfo.Remaining;

            // Build previous attempts summary
            response.PreviousAttempts = await BuildPreviousAttemptsSummary(survey, memberResponse);
        }
        else
        {
            response.TotalAttempts = 0;
            response.CanStartNewAttempt = true;
            response.IsInCooldown = false;
        }

        return response;
    }

    private static (bool IsEligible, string? Reason) CheckEligibility(Survey survey, Response? memberResponse)
    {
        // Check survey state
        if (!survey.IsAcceptingResponses())
        {
            return (false, "Survey is not active");
        }

        // Check time window
        var now = DateTimeOffset.UtcNow;
        if (survey.StartAt.HasValue && now < survey.StartAt.Value)
        {
            return (false, "Survey has not started yet");
        }

        if (survey.EndAt.HasValue && now > survey.EndAt.Value)
        {
            return (false, "Survey has ended");
        }

        // Check attempt limits
        if (memberResponse != null)
        {
            if (memberResponse.AttemptNumber >= survey.ParticipationPolicy.MaxAttemptsPerMember)
            {
                if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
                {
                    return (false, "Maximum attempts reached");
                }
            }
        }

        return (true, null);
    }

    private static string GetAttemptStatus(Response response)
    {
        if (response.SubmittedAt.HasValue)
            return "Submitted";
        
        // Check if response has any answers
        if (response.QuestionAnswers.Any(qa => qa.HasAnswer()))
            return "InProgress";
        
        return "NotStarted";
    }

    private static bool CanStartNewAttempt(Survey survey, Response currentResponse)
    {
        if (!survey.ParticipationPolicy.AllowMultipleSubmissions)
            return false;

        if (currentResponse.AttemptNumber >= survey.ParticipationPolicy.MaxAttemptsPerMember)
            return false;

        if (currentResponse.SubmittedAt.HasValue)
            return true;

        return false; // Current attempt is still in progress
    }

    private static (bool IsInCooldown, DateTime? EndsAt, TimeSpan? Remaining) CheckCooldown(
        Survey survey, 
        Response response)
    {
        if (!survey.ParticipationPolicy.CoolDownSeconds.HasValue)
            return (false, null, null);

        if (!response.SubmittedAt.HasValue)
            return (false, null, null);

        var cooldownEndsAt = response.SubmittedAt.Value.AddSeconds(survey.ParticipationPolicy.CoolDownSeconds.Value);
        var now = DateTimeOffset.UtcNow;

        if (now < cooldownEndsAt)
        {
            return (true, cooldownEndsAt.DateTime, cooldownEndsAt - now);
        }

        return (false, null, null);
    }

    private Task<List<AttemptSummary>> BuildPreviousAttemptsSummary(
        Survey survey, 
        Response currentResponse)
    {
        var attempts = new List<AttemptSummary>();

        // For now, we only have the current response
        // In a real implementation, you might want to store attempt history
        var summary = new AttemptSummary
        {
            ResponseId = currentResponse.Id,
            AttemptNumber = currentResponse.AttemptNumber,
            Status = GetAttemptStatus(currentResponse),
            StartedAt = currentResponse.SubmittedAt?.DateTime,
            SubmittedAt = currentResponse.SubmittedAt?.DateTime,
            AnsweredQuestions = currentResponse.QuestionAnswers.Count(qa => qa.HasAnswer()),
            TotalQuestions = survey.Questions.Count,
            CompletionPercentage = survey.Questions.Count > 0 
                ? (double)currentResponse.QuestionAnswers.Count(qa => qa.HasAnswer()) / survey.Questions.Count * 100 
                : 0
        };

        attempts.Add(summary);
        return Task.FromResult(attempts);
    }
}
