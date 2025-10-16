using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.ValueObjects;
using Nezam.Refahi.Surveying.Domain.Enums;

namespace Nezam.Refahi.Surveying.Domain.Services;

/// <summary>
/// Domain service for survey analytics and calculations
/// Single responsibility: calculating survey metrics and statistics
/// </summary>
public class SurveyAnalyticsService
{
    /// <summary>
    /// محاسبه نرخ تکمیل نظرسنجی
    /// </summary>
    public decimal CalculateCompletionRate(Survey survey)
    {
        var totalResponses = survey.Responses.Count;
        if (totalResponses == 0)
            return 0;

        var completeResponses = survey.Responses.Count(r => r.AttemptStatus == AttemptStatus.Submitted);
        return (decimal)completeResponses / totalResponses * 100;
    }

    /// <summary>
    /// محاسبه نرخ تکمیل نظرسنجی با پاسخ‌های ارائه شده
    /// </summary>
    public decimal CalculateCompletionRate(Survey survey, IEnumerable<Response> responses)
    {
        var responseList = responses.ToList();
        var totalResponses = responseList.Count;
        if (totalResponses == 0)
            return 0;

        var validationService = new SurveyValidationService();
        var completeResponses = responseList.Count(r => validationService.IsResponseComplete(survey, r));
        return (decimal)completeResponses / totalResponses * 100;
    }

    /// <summary>
    /// محاسبه نرخ مشارکت بر اساس اندازه مخاطب هدف
    /// </summary>
    public decimal CalculateParticipationRate(Survey survey, int targetAudienceSize)
    {
        if (targetAudienceSize == 0)
            return 0;

        var uniqueParticipants = survey.Responses
            .Select(r => r.Participant.GetParticipantIdentifier())
            .Distinct()
            .Count();

        return (decimal)uniqueParticipants / targetAudienceSize * 100;
    }

    /// <summary>
    /// محاسبه نرخ مشارکت با پاسخ‌های ارائه شده
    /// </summary>
    public decimal CalculateParticipationRate(Survey survey, int targetAudienceSize, IEnumerable<Response> responses)
    {
        if (targetAudienceSize == 0)
            return 0;

        var uniqueParticipants = responses
            .Select(r => r.Participant.GetParticipantIdentifier())
            .Distinct()
            .Count();

        return (decimal)uniqueParticipants / targetAudienceSize * 100;
    }
}
