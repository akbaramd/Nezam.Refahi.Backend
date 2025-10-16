using Nezam.Refahi.Surveying.Contracts.Dtos;

namespace Nezam.Refahi.Surveying.Application.Services;

/// <summary>
/// Service for mapping server error codes to user-friendly messages
/// </summary>
public interface ISurveyErrorMappingService
{
    /// <summary>
    /// Maps server error message to user-friendly error DTO
    /// </summary>
    SurveyErrorDto MapError(string serverErrorMessage);
    
    /// <summary>
    /// Maps server error message to user-friendly message
    /// </summary>
    string MapToUserMessage(string serverErrorMessage);
    
    /// <summary>
    /// Gets error code from server error message
    /// </summary>
    string? GetErrorCode(string serverErrorMessage);
}

public class SurveyErrorMappingService : ISurveyErrorMappingService
{
    public SurveyErrorDto MapError(string serverErrorMessage)
    {
        var errorCode = GetErrorCode(serverErrorMessage);
        var userMessage = MapToUserMessage(serverErrorMessage);
        
        return new SurveyErrorDto
        {
            Code = errorCode ?? "UNKNOWN_ERROR",
            Message = userMessage,
            UserMessage = userMessage,
            IsRetryable = IsRetryableError(errorCode),
            ActionRequired = GetActionRequired(errorCode)
        };
    }

    public string MapToUserMessage(string serverErrorMessage)
    {
        var errorCode = GetErrorCode(serverErrorMessage);
        
        return errorCode switch
        {
            "REQUIRED_NOT_ANSWERED" => "لطفاً سؤالات الزامی را کامل کنید.",
            "REQUIRED_REPEAT_MISSING" => "تعداد تکرارهای لازم برای این سؤال کامل نیست.",
            "REPEAT_NOT_ALLOWED" => "این سؤال تکرارشونده نیست.",
            "REPEAT_LIMIT_REACHED" => "به سقف تکرار مجاز رسیدید.",
            "BACK_NOT_ALLOWED" => "بازگشت برای این نظرسنجی مجاز نیست.",
            "RESPONSE_IMMUTABLE" => "پاسخ دیگر قابل ویرایش نیست.",
            "SURVEY_NOT_ACTIVE" => "نظرسنجی در حال حاضر فعال نیست.",
            "WINDOW_CLOSED" => "بازه زمانی نظرسنجی به پایان رسیده است.",
            "SURVEY_PAUSED" => "نظرسنجی موقتاً متوقف شده است.",
            "MAX_ATTEMPTS_REACHED" => "سقف تعداد تلاش‌ها تکمیل شده است.",
            "COOLDOWN_ACTIVE" => "لطفاً کمی صبر کنید تا بتوانید تلاش جدیدی شروع کنید.",
            "ACTIVE_ATTEMPT_EXISTS" => "یک تلاش فعال دارید. همین تلاش ادامه می‌یابد.",
            "OPTION_NOT_VALID" => "گزینه انتخاب شده معتبر نیست. لطفاً دوباره انتخاب کنید.",
            "INVALID_REPEAT_INDEX" => "شماره تکرار نامعتبر است.",
            "RESPONSE_ALREADY_SUBMITTED" => "این پاسخ قبلاً ارسال شده است.",
            "SURVEY_NOT_FOUND" => "نظرسنجی یافت نشد.",
            "RESPONSE_NOT_FOUND" => "پاسخ یافت نشد.",
            "QUESTION_NOT_FOUND" => "سؤال یافت نشد.",
            "UNAUTHORIZED" => "دسترسی غیرمجاز.",
            "FORBIDDEN" => "شما مجاز به انجام این عمل نیستید.",
            _ => "خطایی رخ داده است. لطفاً دوباره تلاش کنید."
        };
    }

    public string? GetErrorCode(string serverErrorMessage)
    {
        if (string.IsNullOrEmpty(serverErrorMessage))
            return null;

        // Extract error code from messages like "REQUIRED_NOT_ANSWERED: سوالات اجباری..."
        var colonIndex = serverErrorMessage.IndexOf(':');
        if (colonIndex > 0)
        {
            var code = serverErrorMessage.Substring(0, colonIndex).Trim();
            if (IsValidErrorCode(code))
                return code;
        }

        // Check if the entire message is an error code
        if (IsValidErrorCode(serverErrorMessage))
            return serverErrorMessage;

        return null;
    }

    private static bool IsValidErrorCode(string code)
    {
        var validCodes = new[]
        {
            "REQUIRED_NOT_ANSWERED", "REQUIRED_REPEAT_MISSING", "REPEAT_NOT_ALLOWED",
            "REPEAT_LIMIT_REACHED", "BACK_NOT_ALLOWED", "RESPONSE_IMMUTABLE",
            "SURVEY_NOT_ACTIVE", "WINDOW_CLOSED", "SURVEY_PAUSED", "MAX_ATTEMPTS_REACHED",
            "COOLDOWN_ACTIVE", "ACTIVE_ATTEMPT_EXISTS", "OPTION_NOT_VALID",
            "INVALID_REPEAT_INDEX", "RESPONSE_ALREADY_SUBMITTED", "SURVEY_NOT_FOUND",
            "RESPONSE_NOT_FOUND", "QUESTION_NOT_FOUND", "UNAUTHORIZED", "FORBIDDEN"
        };

        return validCodes.Contains(code);
    }

    private static bool IsRetryableError(string? errorCode)
    {
        return errorCode switch
        {
            "SURVEY_NOT_ACTIVE" => true, // Survey might become active
            "COOLDOWN_ACTIVE" => true,   // Cooldown will expire
            "OPTION_NOT_VALID" => true,  // Options might be reloaded
            _ => false
        };
    }

    private static string? GetActionRequired(string? errorCode)
    {
        return errorCode switch
        {
            "REQUIRED_NOT_ANSWERED" => "ANSWER_REQUIRED_QUESTIONS",
            "REQUIRED_REPEAT_MISSING" => "COMPLETE_REQUIRED_REPEATS",
            "BACK_NOT_ALLOWED" => "DISABLE_BACK_NAVIGATION",
            "REPEAT_LIMIT_REACHED" => "DISABLE_ADD_REPEAT",
            "MAX_ATTEMPTS_REACHED" => "DISABLE_START_NEW_ATTEMPT",
            "COOLDOWN_ACTIVE" => "SHOW_COOLDOWN_TIMER",
            "ACTIVE_ATTEMPT_EXISTS" => "RESUME_EXISTING_ATTEMPT",
            _ => null
        };
    }
}

/// <summary>
/// DTO for structured error information
/// </summary>
public class SurveyErrorDto
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
    public bool IsRetryable { get; set; }
    public string? ActionRequired { get; set; }
    public Dictionary<string, object>? Details { get; set; }
}

