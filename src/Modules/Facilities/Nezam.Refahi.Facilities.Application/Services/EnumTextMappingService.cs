using Nezam.Refahi.Facilities.Domain.Enums;

namespace Nezam.Refahi.Facilities.Application.Services;

/// <summary>
/// Service for mapping enum values to human-readable text descriptions
/// Follows enterprise patterns for internationalization and localization
/// </summary>
public static class EnumTextMappingService
{


    /// <summary>
    /// Maps FacilityCycleStatus enum to human-readable description
    /// </summary>
    public static string GetFacilityCycleStatusDescription(FacilityCycleStatus status)
    {
        return status switch
        {
            FacilityCycleStatus.Draft => "پیش‌نویس",
            FacilityCycleStatus.Active => "فعال",
            FacilityCycleStatus.Closed => "بسته شده",
            FacilityCycleStatus.UnderReview => "تحت بررسی",
            FacilityCycleStatus.Completed => "تکمیل شده",
            FacilityCycleStatus.Cancelled => "لغو شده",
            _ => "نامشخص"
        };
    }

    /// <summary>
    /// Maps FacilityRequestStatus enum to human-readable description
    /// </summary>
    public static string GetFacilityRequestStatusDescription(FacilityRequestStatus status)
    {
        return status switch
        {
            FacilityRequestStatus.RequestSent => "درخواست ارسال شده",
            FacilityRequestStatus.PendingApproval => "در انتظار تأیید",
            FacilityRequestStatus.PendingDocuments => "در انتظار مدارک",
            FacilityRequestStatus.Waitlisted => "در انتظار لیست",
            FacilityRequestStatus.ReturnedForAmendment => "برگشت برای اصلاح",
            FacilityRequestStatus.UnderReview => "تحت بررسی",
            FacilityRequestStatus.Approved => "تأیید شده",
            FacilityRequestStatus.Rejected => "رد شده",
            FacilityRequestStatus.Cancelled => "لغو شده",
            FacilityRequestStatus.QueuedForDispatch => "در صف اعزام",
            FacilityRequestStatus.SentToBank => "اعزام شده به بانک",
            FacilityRequestStatus.BankScheduled => "زمان‌بندی شده در بانک",
            FacilityRequestStatus.ProcessedByBank => "پردازش شده توسط بانک",
            FacilityRequestStatus.Completed => "تکمیل شده",
            FacilityRequestStatus.Disbursed => "پرداخت شده",
            FacilityRequestStatus.Expired => "منقضی شده",
            FacilityRequestStatus.BankCancelled => "لغو شده توسط بانک",
            _ => "نامشخص"
        };
    }

    /// <summary>
    /// Maps admission strategy to human-readable description
    /// </summary>
    public static string GetAdmissionStrategyDescription(string strategy)
    {
        return strategy?.ToUpperInvariant() switch
        {
            "FIFO" => "اولویت زمانی (اولین ورود، اولین خروج)",
            "SCORE" => "اولویت امتیازی",
            "LOTTERY" => "قرعه‌کشی",
            "PRIORITY" => "اولویت‌بندی",
            _ => "نامشخص"
        };
    }


    /// <summary>
    /// Determines if a cycle status indicates the cycle is active
    /// </summary>
    public static bool IsCycleActive(FacilityCycleStatus status)
    {
        return status == FacilityCycleStatus.Active;
    }

    /// <summary>
    /// Determines if a request status indicates the request is in progress
    /// </summary>
    public static bool IsRequestInProgress(FacilityRequestStatus status)
    {
        return status switch
        {
            FacilityRequestStatus.RequestSent or
            FacilityRequestStatus.PendingApproval or
            FacilityRequestStatus.PendingDocuments or
            FacilityRequestStatus.Waitlisted or
            FacilityRequestStatus.ReturnedForAmendment or
            FacilityRequestStatus.UnderReview or
            FacilityRequestStatus.Approved or
            FacilityRequestStatus.QueuedForDispatch or
            FacilityRequestStatus.SentToBank or
            FacilityRequestStatus.BankScheduled or
            FacilityRequestStatus.ProcessedByBank => true,
            _ => false
        };
    }

    /// <summary>
    /// Determines if a request status indicates the request is completed
    /// </summary>
    public static bool IsRequestCompleted(FacilityRequestStatus status)
    {
        return status == FacilityRequestStatus.Completed || status == FacilityRequestStatus.Disbursed;
    }

    /// <summary>
    /// Determines if a request status indicates the request is rejected
    /// </summary>
    public static bool IsRequestRejected(FacilityRequestStatus status)
    {
        return status == FacilityRequestStatus.Rejected;
    }

    /// <summary>
    /// Determines if a request status indicates the request is cancelled
    /// </summary>
    public static bool IsRequestCancelled(FacilityRequestStatus status)
    {
        return status == FacilityRequestStatus.Cancelled || status == FacilityRequestStatus.BankCancelled;
    }

    /// <summary>
    /// Determines if a request status indicates the request requires applicant action
    /// </summary>
    public static bool RequiresApplicantAction(FacilityRequestStatus status)
    {
        return status switch
        {
            FacilityRequestStatus.PendingDocuments or
            FacilityRequestStatus.ReturnedForAmendment => true,
            _ => false
        };
    }

    /// <summary>
    /// Determines if a request status indicates the request requires bank action
    /// </summary>
    public static bool RequiresBankAction(FacilityRequestStatus status)
    {
        return status switch
        {
            FacilityRequestStatus.QueuedForDispatch or
            FacilityRequestStatus.SentToBank or
            FacilityRequestStatus.BankScheduled or
            FacilityRequestStatus.ProcessedByBank => true,
            _ => false
        };
    }

    /// <summary>
    /// Determines if a request status is terminal (cannot be changed)
    /// </summary>
    public static bool IsRequestTerminal(FacilityRequestStatus status)
    {
        return status switch
        {
            FacilityRequestStatus.Completed or
            FacilityRequestStatus.Disbursed or
            FacilityRequestStatus.Rejected or
            FacilityRequestStatus.Cancelled or
            FacilityRequestStatus.BankCancelled or
            FacilityRequestStatus.Expired => true,
            _ => false
        };
    }

    /// <summary>
    /// Determines if a request can be cancelled
    /// </summary>
    public static bool CanRequestBeCancelled(FacilityRequestStatus status)
    {
        return status switch
        {
            FacilityRequestStatus.PendingApproval or
            FacilityRequestStatus.PendingDocuments or
            FacilityRequestStatus.Waitlisted or
            FacilityRequestStatus.ReturnedForAmendment or
            FacilityRequestStatus.UnderReview or
            FacilityRequestStatus.Approved or
            FacilityRequestStatus.QueuedForDispatch => true,
            _ => false
        };
    }
}
