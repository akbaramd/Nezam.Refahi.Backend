namespace Nezam.Refahi.Surveying.Domain.Enums;

/// <summary>
/// Helper class for ResponseStatus enum operations
/// </summary>
public static class ResponseStatusHelper
{
    /// <summary>
    /// Gets the Persian text for a ResponseStatus
    /// </summary>
    public static string GetPersianText(ResponseStatus status)
    {
        return status switch
        {
            ResponseStatus.Answering => "در حال جواب",
            ResponseStatus.Reviewing => "در حال ریویو",
            ResponseStatus.Completed => "اتمام شده",
            ResponseStatus.Cancelled => "لغو شده",
            ResponseStatus.Expired => "منقضی شده",
            _ => "نامشخص"
        };
    }

    /// <summary>
    /// Gets the English text for a ResponseStatus
    /// </summary>
    public static string GetEnglishText(ResponseStatus status)
    {
        return status switch
        {
            ResponseStatus.Answering => "Answering",
            ResponseStatus.Reviewing => "Reviewing",
            ResponseStatus.Completed => "Completed",
            ResponseStatus.Cancelled => "Cancelled",
            ResponseStatus.Expired => "Expired",
            _ => "Unknown"
        };
    }
}
