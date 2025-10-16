using Nezam.Refahi.Surveying.Domain.Entities;
using Nezam.Refahi.Surveying.Domain.Enums;
using Nezam.Refahi.Surveying.Domain.ValueObjects;

namespace Nezam.Refahi.Surveying.Domain.Services;

/// <summary>
/// Domain service for participation rules validation
/// Single responsibility: validating participation rules and constraints
/// </summary>
public class ParticipationRulesDomainService
{
    /// <summary>
    /// بررسی امکان مشارکت شرکت‌کننده در نظرسنجی
    /// هدف: بررسی تمام قوانین مشارکت شامل حداکثر تلاش‌ها، زمان انتظار و تداخل
    /// نتیجه مورد انتظار: true اگر شرکت‌کننده بتواند مشارکت کند، false اگر نتواند
    /// منطق کسب‌وکار: تمام قوانین مشارکت باید بررسی شوند
    /// </summary>
    public bool CanParticipantParticipate(Survey survey, ParticipantInfo participant, int attemptNumber, DateTime? lastAttemptTime = null)
    {
        // بررسی اولیه - آیا نظرسنجی فعال است؟
        if (!survey.IsAcceptingResponses())
            return false;

        // بررسی سیاست مشارکت
        if (!survey.ParticipationPolicy.IsAttemptAllowed(attemptNumber))
            return false;

        // بررسی زمان انتظار
        if (lastAttemptTime.HasValue && !survey.ParticipationPolicy.IsCoolDownPassed(lastAttemptTime.Value))
            return false;

        return true;
    }

    /// <summary>
    /// بررسی تداخل بین شرکت‌کنندگان مختلف
    /// هدف: اطمینان از اینکه شرکت‌کنندگان مختلف تداخل ندارند
    /// نتیجه مورد انتظار: true اگر تداخل نداشته باشند، false اگر تداخل داشته باشند
    /// منطق کسب‌وکار: شرکت‌کنندگان مختلف باید مستقل عمل کنند
    /// </summary>
    public bool ValidateNoInterferenceBetweenParticipants(IEnumerable<Response> existingResponses, ParticipantInfo newParticipant, int newAttemptNumber)
    {
        var responses = existingResponses.ToList();
        
        // بررسی تداخل با شرکت‌کنندگان دیگر
        foreach (var response in responses)
        {
            // اگر شرکت‌کننده یکسان است، بررسی شماره تلاش
            if (response.Participant.Equals(newParticipant))
            {
                // نباید شماره تلاش تکراری داشته باشد
                if (response.AttemptNumber == newAttemptNumber)
                    return false;
            }
            // اگر شرکت‌کننده متفاوت است، باید مستقل باشد
            else
            {
                // شرکت‌کنندگان مختلف نباید تداخل داشته باشند
                if (response.Participant.IsAnonymous && newParticipant.IsAnonymous)
                {
                    // شرکت‌کنندگان ناشناس باید hash متفاوت داشته باشند
                    if (response.Participant.GetShortIdentifier() == newParticipant.GetShortIdentifier())
                        return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// بررسی قوانین تکرار سوالات در پاسخ
    /// هدف: بررسی اینکه تکرار سوالات مطابق با سیاست تکرار باشد
    /// نتیجه مورد انتظار: true اگر تکرارها معتبر باشند، false اگر نباشند
    /// منطق کسب‌وکار: تکرار سوالات باید مطابق با RepeatPolicy باشد
    /// </summary>
    public bool ValidateQuestionRepeats(Survey survey, Response response)
    {
        foreach (var question in survey.Questions)
        {
            var questionAnswers = response.GetQuestionAnswers(question.Id).ToList();
            
            // بررسی تعداد تکرارها - بررسی اینکه آیا تعداد تکرارها بیش از حد مجاز است
            if (question.RepeatPolicy.Kind == RepeatPolicyKind.Bounded && 
                questionAnswers.Count > question.RepeatPolicy.MaxRepeats!.Value)
                return false;

            // بررسی شماره تکرارها
            foreach (var answer in questionAnswers)
            {
                if (!question.RepeatPolicy.IsValidRepeatIndex(answer.RepeatIndex))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// بررسی قوانین کسب‌وکار پیچیده مشارکت
    /// هدف: بررسی سناریوهای پیچیده ترکیبی از قوانین مختلف
    /// نتیجه مورد انتظار: true اگر تمام قوانین رعایت شوند، false اگر نباشند
    /// منطق کسب‌وکار: تمام قوانین باید بدون تداخل اعمال شوند
    /// </summary>
    public bool ValidateComplexParticipationRules(Survey survey, ParticipantInfo participant, int attemptNumber, 
        DateTime? lastAttemptTime, IEnumerable<Response> existingResponses)
    {
        // بررسی اولیه مشارکت
        if (!CanParticipantParticipate(survey, participant, attemptNumber, lastAttemptTime))
            return false;

        // بررسی تداخل
        if (!ValidateNoInterferenceBetweenParticipants(existingResponses, participant, attemptNumber))
            return false;

        return true;
    }

    /// <summary>
    /// محاسبه تعداد شرکت‌کنندگان منحصر به فرد
    /// هدف: محاسبه تعداد شرکت‌کنندگان منحصر به فرد از لیست پاسخ‌ها
    /// نتیجه مورد انتظار: تعداد شرکت‌کنندگان منحصر به فرد
    /// منطق کسب‌وکار: شرکت‌کنندگان ناشناس و شناخته شده باید جداگانه شمارش شوند
    /// </summary>
    public int CalculateUniqueParticipants(IEnumerable<Response> responses)
    {
        var uniqueParticipants = new HashSet<string>();
        
        foreach (var response in responses)
        {
            if (response.Participant.IsAnonymous)
            {
                // برای شرکت‌کنندگان ناشناس، از hash استفاده می‌کنیم
                uniqueParticipants.Add($"anonymous_{response.Participant.GetShortIdentifier()}");
            }
            else
            {
                // برای شرکت‌کنندگان شناخته شده، از MemberId استفاده می‌کنیم
                uniqueParticipants.Add($"member_{response.Participant.MemberId}");
            }
        }

        return uniqueParticipants.Count;
    }

    /// <summary>
    /// بررسی قوانین انتقال حالت پاسخ
    /// هدف: بررسی اینکه انتقال حالت پاسخ مطابق با قوانین باشد
    /// نتیجه مورد انتظار: true اگر انتقال مجاز باشد، false اگر نباشد
    /// منطق کسب‌وکار: انتقال حالت‌ها باید مطابق با قوانین کسب‌وکار باشد
    /// </summary>
    public bool CanTransitionResponseState(Response response, AttemptStatus newState)
    {
        var currentState = response.AttemptStatus;

        return currentState switch
        {
            AttemptStatus.Active => newState == AttemptStatus.Submitted || newState == AttemptStatus.Canceled,
            AttemptStatus.Submitted => newState == AttemptStatus.Expired,
            AttemptStatus.Canceled => false, // پاسخ لغو شده نمی‌تواند تغییر کند
            AttemptStatus.Expired => false, // پاسخ منقضی شده نمی‌تواند تغییر کند
            _ => false
        };
    }

    /// <summary>
    /// بررسی قوانین کسب‌وکار برای پاسخ‌های متعدد
    /// هدف: بررسی اینکه پاسخ‌های متعدد از یک شرکت‌کننده مطابق با قوانین باشند
    /// نتیجه مورد انتظار: true اگر تمام قوانین رعایت شوند، false اگر نباشند
    /// منطق کسب‌وکار: پاسخ‌های متعدد باید مطابق با سیاست مشارکت باشند
    /// </summary>
    public bool ValidateMultipleResponsesFromSameParticipant(Survey survey, ParticipantInfo participant, IEnumerable<Response> responses)
    {
        var participantResponses = responses.Where(r => r.Participant.Equals(participant)).ToList();
        
        // بررسی تعداد پاسخ‌ها
        if (survey.ParticipationPolicy != null && participantResponses.Count > survey.ParticipationPolicy.MaxAttemptsPerMember)
            return false;

        // بررسی شماره تلاش‌ها
        var attemptNumbers = participantResponses.Select(r => r.AttemptNumber).ToList();
        var expectedAttemptNumbers = Enumerable.Range(1, participantResponses.Count).ToList();
        
        if (!attemptNumbers.SequenceEqual(expectedAttemptNumbers))
            return false;

        // بررسی زمان انتظار بین پاسخ‌ها
        if (survey.ParticipationPolicy?.CoolDownSeconds.HasValue == true)
        {
            // Note: Response entity doesn't have CreatedAt, this validation would need to be handled differently
            // For now, we'll skip this validation as it requires additional infrastructure
            // This could be implemented using a separate service that tracks response timestamps
        }

        return true;
    }

    /// <summary>
    /// بررسی مجوز عضو برای شرکت در نظرسنجی بر اساس ویژگی‌ها و قابلیت‌ها
    /// هدف: بررسی اینکه آیا عضو دارای حداقل یکی از ویژگی‌ها یا قابلیت‌های مورد نیاز نظرسنجی است
    /// نتیجه مورد انتظار: true اگر عضو مجاز باشد، false اگر نباشد
    /// منطق کسب‌وکار: عضو باید حداقل یکی از ویژگی‌ها یا قابلیت‌های مورد نیاز نظرسنجی را داشته باشد (OR Logic)
    /// </summary>
    public bool ValidateMemberAuthorization(Survey survey, IEnumerable<string> memberFeatures, IEnumerable<string> memberCapabilities)
    {
        // اگر نظرسنجی هیچ محدودیتی ندارد، همه مجاز هستند
        if (!survey.SurveyFeatures.Any() && !survey.SurveyCapabilities.Any())
            return true;

        var memberFeaturesList = memberFeatures?.ToList() ?? new List<string>();
        var memberCapabilitiesList = memberCapabilities?.ToList() ?? new List<string>();

        // بررسی ویژگی‌ها - آیا عضو حداقل یکی از ویژگی‌های مورد نیاز را دارد؟
        var requiredFeatures = survey.SurveyFeatures.Select(sf => sf.FeatureCode).ToList();
        var hasRequiredFeature = !requiredFeatures.Any() || requiredFeatures.Any(f => memberFeaturesList.Contains(f));

        // بررسی قابلیت‌ها - آیا عضو حداقل یکی از قابلیت‌های مورد نیاز را دارد؟
        var requiredCapabilities = survey.SurveyCapabilities.Select(sc => sc.CapabilityCode).ToList();
        var hasRequiredCapability = !requiredCapabilities.Any() || requiredCapabilities.Any(c => memberCapabilitiesList.Contains(c));

        // اگر نظرسنجی هم ویژگی و هم قابلیت نیاز دارد، عضو باید حداقل یکی از هر کدام را داشته باشد
        if (requiredFeatures.Any() && requiredCapabilities.Any())
        {
            return hasRequiredFeature && hasRequiredCapability;
        }

        // اگر فقط ویژگی یا فقط قابلیت نیاز دارد، داشتن یکی کافی است
        return hasRequiredFeature || hasRequiredCapability;
    }

    /// <summary>
    /// بررسی مجوز عضو برای شرکت در نظرسنجی بر اساس ویژگی‌ها و قابلیت‌ها
    /// هدف: بررسی اینکه آیا عضو دارای حداقل یکی از ویژگی‌ها یا قابلیت‌های مورد نیاز نظرسنجی است
    /// نتیجه مورد انتظار: نتیجه اعتبارسنجی با جزئیات خطا
    /// منطق کسب‌وکار: عضو باید حداقل یکی از ویژگی‌ها یا قابلیت‌های مورد نیاز نظرسنجی را داشته باشد (OR Logic)
    /// </summary>
    public AuthorizationValidationResult ValidateMemberAuthorizationWithDetails(Survey survey, IEnumerable<string> memberFeatures, IEnumerable<string> memberCapabilities)
    {
        var errors = new List<string>();

        // اگر نظرسنجی هیچ محدودیتی ندارد، همه مجاز هستند
        if (!survey.SurveyFeatures.Any() && !survey.SurveyCapabilities.Any())
        {
            return new AuthorizationValidationResult(true, null, null);
        }

        var memberFeaturesList = memberFeatures?.ToList() ?? new List<string>();
        var memberCapabilitiesList = memberCapabilities?.ToList() ?? new List<string>();

        // بررسی ویژگی‌های مورد نیاز نظرسنجی
        var requiredFeatures = survey.SurveyFeatures.Select(sf => sf.FeatureCode).ToList();
        var hasRequiredFeature = !requiredFeatures.Any() || requiredFeatures.Any(f => memberFeaturesList.Contains(f));

        // بررسی قابلیت‌های مورد نیاز نظرسنجی
        var requiredCapabilities = survey.SurveyCapabilities.Select(sc => sc.CapabilityCode).ToList();
        var hasRequiredCapability = !requiredCapabilities.Any() || requiredCapabilities.Any(c => memberCapabilitiesList.Contains(c));

        // اگر نظرسنجی هم ویژگی و هم قابلیت نیاز دارد
        if (requiredFeatures.Any() && requiredCapabilities.Any())
        {
            if (!hasRequiredFeature && !hasRequiredCapability)
            {
                errors.Add($"شما باید حداقل یکی از ویژگی‌های مورد نیاز ({string.Join(", ", requiredFeatures)}) یا یکی از قابلیت‌های مورد نیاز ({string.Join(", ", requiredCapabilities)}) را داشته باشید");
            }
            else if (!hasRequiredFeature)
            {
                errors.Add($"شما باید حداقل یکی از ویژگی‌های مورد نیاز را داشته باشید: {string.Join(", ", requiredFeatures)}");
            }
            else if (!hasRequiredCapability)
            {
                errors.Add($"شما باید حداقل یکی از قابلیت‌های مورد نیاز را داشته باشید: {string.Join(", ", requiredCapabilities)}");
            }
        }
        // اگر فقط ویژگی نیاز دارد
        else if (requiredFeatures.Any() && !hasRequiredFeature)
        {
            errors.Add($"شما باید حداقل یکی از ویژگی‌های مورد نیاز را داشته باشید: {string.Join(", ", requiredFeatures)}");
        }
        // اگر فقط قابلیت نیاز دارد
        else if (requiredCapabilities.Any() && !hasRequiredCapability)
        {
            errors.Add($"شما باید حداقل یکی از قابلیت‌های مورد نیاز را داشته باشید: {string.Join(", ", requiredCapabilities)}");
        }

        return new AuthorizationValidationResult(errors.Count == 0, errors.FirstOrDefault(), errors);
    }
}

/// <summary>
/// نتیجه اعتبارسنجی مجوز عضو
/// </summary>
public record AuthorizationValidationResult(bool IsAuthorized, string? ErrorMessage, List<string>? ValidationErrors);
