using System.Text.Json.Serialization;

namespace Nezam.Refahi.Facilities.Domain.Enums;

/// <summary>
/// وضعیت‌های درخواست تسهیلات
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FacilityRequestStatus
{
    /// <summary>
    /// درخواست ارسال شده - درخواست توسط کاربر ارسال شده
    /// </summary>
    RequestSent = 1,

    /// <summary>
    /// در انتظار تأیید - در انتظار بررسی و تأیید/رد
    /// </summary>
    PendingApproval = 2,

    /// <summary>
    /// در انتظار مدارک - نیاز به تکمیل مدارک
    /// </summary>
    PendingDocuments = 3,

    /// <summary>
    /// در انتظار لیست - در لیست انتظار قرار گرفته
    /// </summary>
    Waitlisted = 4,

    /// <summary>
    /// برگشت برای اصلاح - نیاز به اصلاح اطلاعات
    /// </summary>
    ReturnedForAmendment = 5,

    /// <summary>
    /// تحت بررسی - در حال بررسی توسط مسئول
    /// </summary>
    UnderReview = 6,

    /// <summary>
    /// تأیید شده - درخواست تأیید شده
    /// </summary>
    Approved = 7,

    /// <summary>
    /// رد شده - درخواست رد شده
    /// </summary>
    Rejected = 8,

    /// <summary>
    /// لغو شده - درخواست لغو شده
    /// </summary>
    Cancelled = 9,

    /// <summary>
    /// در صف اعزام - آماده اعزام به بانک
    /// </summary>
    QueuedForDispatch = 10,

    /// <summary>
    /// اعزام شده به بانک - به بانک ارسال شده
    /// </summary>
    SentToBank = 11,

    /// <summary>
    /// زمان‌بندی شده در بانک - قرار ملاقات تنظیم شده
    /// </summary>
    BankScheduled = 12,

    /// <summary>
    /// پردازش شده توسط بانک - بانک پردازش کرده
    /// </summary>
    ProcessedByBank = 13,

    /// <summary>
    /// تکمیل شده - فرآیند کامل شده
    /// </summary>
    Completed = 14,

    /// <summary>
    /// پرداخت شده - مبلغ پرداخت شده
    /// </summary>
    Disbursed = 15,

    /// <summary>
    /// منقضی شده - قرار ملاقات منقضی شده
    /// </summary>
    Expired = 16,

    /// <summary>
    /// لغو شده توسط بانک - بانک لغو کرده
    /// </summary>
    BankCancelled = 17
}
