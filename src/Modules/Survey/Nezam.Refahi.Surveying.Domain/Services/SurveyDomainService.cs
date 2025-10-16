using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Domain.Services;

/// <summary>
/// Domain service for survey participation validation
/// Single responsibility: validating participant eligibility
/// </summary>
public class SurveyParticipationService
{
    /// <summary>
    /// بررسی واجد شرایط بودن شرکت‌کننده برای نظرسنجی بر اساس فیلتر مخاطب
    /// </summary>
    public bool IsParticipantEligible(Survey survey, ParticipantInfo participant, Dictionary<string, object> memberDemographics)
    {
        if (survey.AudienceFilter == null || survey.AudienceFilter.IsEmpty())
            return true; // No restrictions

        // For anonymous surveys, we can't validate against member demographics
        if (participant.IsAnonymous)
            return true;

        // Evaluate audience filter against member demographics
        return EvaluateAudienceFilter(survey.AudienceFilter, memberDemographics);
    }

    /// <summary>
    /// ارزیابی فیلتر مخاطب بر اساس اطلاعات دموگرافیک عضو
    /// </summary>
    private bool EvaluateAudienceFilter(AudienceFilter filter, Dictionary<string, object> demographics)
    {
        try
        {
            var criteria = filter.GetCriteria();
            
            foreach (var (key, value) in criteria)
            {
                if (!demographics.TryGetValue(key, out var memberValue))
                    return false;

                if (!EvaluateCriterion(key, value, memberValue))
                    return false;
            }

            return true;
        }
        catch
        {
            return false; // Invalid filter expression
        }
    }

    /// <summary>
    /// ارزیابی یک معیار واحد
    /// </summary>
    private bool EvaluateCriterion(string key, object filterValue, object memberValue)
    {
        // Handle JsonElement from AudienceFilter
        if (filterValue is System.Text.Json.JsonElement filterElement && memberValue is List<string> memberList)
        {
            if (filterElement.ValueKind == System.Text.Json.JsonValueKind.Array)
            {
                var jsonFilterList = filterElement.EnumerateArray()
                    .Select(x => x.GetString())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToList()!;
                
                return jsonFilterList.All(filterItem => memberList.Contains(filterItem!));
            }
        }
        
        // Handle List<string> comparisons
        if (filterValue is List<string> directFilterList && memberValue is List<string> directMemberList)
        {
            return directFilterList.All(filterItem => directMemberList.Contains(filterItem));
        }
        
        // Simple equality check for other types
        return filterValue.ToString() == memberValue.ToString();
    }
}
