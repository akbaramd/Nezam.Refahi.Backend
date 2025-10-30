using System.Text.Json.Serialization;

namespace Nezam.Refahi.Facilities.Domain.Enums;

/// <summary>
/// انواع رد درخواست تسهیلات
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FacilityRejectionType
{
    /// <summary>
    /// رد عمومی
    /// </summary>
    General = 0,
    
    /// <summary>
    /// رد به دلیل عدم واجد شرایط بودن
    /// </summary>
    Eligibility = 1,
    
    /// <summary>
    /// رد به دلیل مدارک ناقص
    /// </summary>
    IncompleteDocuments = 2,
    
    /// <summary>
    /// رد به دلیل عدم توانایی مالی
    /// </summary>
    FinancialInability = 3,
    
    /// <summary>
    /// رد به دلیل عدم رعایت قوانین
    /// </summary>
    PolicyViolation = 4,
    
    /// <summary>
    /// رد به دلیل تکمیل ظرفیت
    /// </summary>
    CapacityFull = 5,
    
    /// <summary>
    /// رد به دلیل عدم ارائه طرح کسب و کار
    /// </summary>
    MissingBusinessPlan = 6,
    
    /// <summary>
    /// رد به دلیل عدم تأیید بانک
    /// </summary>
    BankRejection = 7
}
