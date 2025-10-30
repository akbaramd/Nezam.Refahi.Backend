namespace Nezam.Refahi.Surveying.Domain.Enums;

/// <summary>
/// Response status indicating the current state of a survey response
/// </summary>
public enum ResponseStatus
{
    /// <summary>
    /// Response is being answered (در حال جواب)
    /// </summary>
    Answering = 1,
    
    /// <summary>
    /// Response is under review (در حال ریویو)
    /// </summary>
    Reviewing = 2,
    
    /// <summary>
    /// Response is completed (اتمام شده)
    /// </summary>
    Completed = 3,
    
    /// <summary>
    /// Response is cancelled (لغو شده)
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// Response is expired (منقضی شده)
    /// </summary>
    Expired = 5
}
