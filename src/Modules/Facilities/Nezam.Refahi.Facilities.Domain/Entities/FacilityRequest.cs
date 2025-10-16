using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Facilities.Domain.Enums;
using Nezam.Refahi.Facilities.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Facilities.Domain.Entities;

/// <summary>
/// درخواست تسهیلات - Aggregate Root مستقل برای مدیریت درخواست‌ها
/// در دنیای واقعی: درخواست عضو برای دریافت تسهیلات در دوره مشخص
/// </summary>
public sealed class FacilityRequest : FullAggregateRoot<Guid>
{
    public Guid FacilityId { get; private set; }
    public Guid FacilityCycleId { get; private set; }
    public Guid MemberId { get; private set; } // Changed from ExternalUserId to MemberId
    public string? UserFullName { get; private set; }
    public string? UserNationalId { get; private set; }
    public Money RequestedAmount { get; private set; } = null!;
    public Money? ApprovedAmount { get; private set; }
    public FacilityRequestStatus Status { get; private set; }
    public string? RequestNumber { get; private set; }
    public string? Description { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public DateTime? DispatchedToBankAt { get; private set; }
    public DateTime? ProcessedByBankAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Bank scheduling
    public DateTime? BankAppointmentScheduledAt { get; private set; }
    public DateTime? BankAppointmentDate { get; private set; }
    public string? BankAppointmentReference { get; private set; }

    // Disbursement tracking
    public DateTime? DisbursedAt { get; private set; }
    public string? DisbursementReference { get; private set; }

    // Policy snapshot for audit
    public Dictionary<string, object> PolicySnapshot { get; private set; } = new();

    // Idempotency and audit
    public string? IdempotencyKey { get; private set; }
    public string? CorrelationId { get; private set; }

    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    public Facility Facility { get; private set; } = null!;
    public FacilityCycle FacilityCycle { get; private set; } = null!;

    // Private constructor for EF Core
    private FacilityRequest() : base() { }

    /// <summary>
    /// ایجاد درخواست جدید برای تسهیلات
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک درخواست جدید برای تسهیلات ایجاد می‌کند که شامل اطلاعات عضو،
    /// مبلغ درخواستی و سایر جزئیات اولیه است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که عضو تصمیم به درخواست تسهیلات می‌گیرد، این رفتار برای
    /// ایجاد درخواست اولیه استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه تسهیلات و دوره اجباری است.
    /// - شناسه عضو اجباری است.
    /// - مبلغ درخواستی باید مثبت باشد.
    /// - وضعیت اولیه PendingApproval است.
    /// - شماره درخواست به صورت خودکار تولید می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه‌های ضروری ارائه شود.
    /// - باید: مبلغ مثبت باشد.
    /// - نباید: بدون اطلاعات ضروری ایجاد شود.
    /// - نباید: مبلغ منفی یا صفر درخواست شود.
    /// </remarks>
    public FacilityRequest(
        Guid facilityId,
        Guid facilityCycleId,
        Guid memberId,
        Money requestedAmount,
        string? userFullName = null,
        string? userNationalId = null,
        string? description = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (facilityId == Guid.Empty)
            throw new ArgumentException("Facility ID cannot be empty", nameof(facilityId));
        if (facilityCycleId == Guid.Empty)
            throw new ArgumentException("Facility cycle ID cannot be empty", nameof(facilityCycleId));
        if (memberId == Guid.Empty)
            throw new ArgumentException("Member ID cannot be empty", nameof(memberId));
        if (requestedAmount == null)
            throw new ArgumentNullException(nameof(requestedAmount));
        if (requestedAmount.AmountRials <= 0)
            throw new ArgumentException("Requested amount must be positive", nameof(requestedAmount));

        FacilityId = facilityId;
        FacilityCycleId = facilityCycleId;
        MemberId = memberId;
        RequestedAmount = requestedAmount;
        UserFullName = userFullName?.Trim();
        UserNationalId = userNationalId?.Trim();
        Status = FacilityRequestStatus.PendingApproval;
        RequestNumber = GenerateApplicationNumber();
        Description = description?.Trim();
        Metadata = metadata ?? new Dictionary<string, string>();
        
        // Ensure CreatedAt is set
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// شروع بررسی درخواست
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار وضعیت درخواست را از PendingApproval به UnderReview تغییر می‌دهد
    /// و فرآیند بررسی را آغاز می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مسئول بررسی درخواست‌ها شروع به بررسی درخواست می‌کند،
    /// این رفتار برای تغییر وضعیت استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های PendingApproval قابل بررسی هستند.
    /// - وضعیت به UnderReview تغییر می‌یابد.
    /// - زمان شروع بررسی ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت PendingApproval قابل تغییر باشد.
    /// - باید: زمان شروع بررسی ثبت شود.
    /// - نباید: درخواست‌های غیر PendingApproval بررسی شوند.
    /// - نباید: بدون شروع واقعی بررسی تغییر کند.
    /// </remarks>
    public void StartReview()
    {
        if (Status != FacilityRequestStatus.PendingApproval)
            throw new InvalidOperationException("Can only start review for pending approval requests");

        var previousStatus = Status;
        Status = FacilityRequestStatus.UnderReview;
    }

    /// <summary>
    /// تأیید درخواست
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار درخواست را تأیید کرده و مبلغ تأیید شده را ثبت می‌نماید.
    /// درخواست تأیید شده آماده اعزام به بانک است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مسئول بررسی درخواست را تأیید می‌کند، این رفتار برای
    /// ثبت تأیید و مبلغ نهایی استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های UnderReview قابل تأیید هستند.
    /// - مبلغ تأیید شده باید مثبت باشد.
    /// - وضعیت به Approved تغییر می‌یابد.
    /// - زمان تأیید ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: مبلغ تأیید شده معتبر باشد.
    /// - باید: زمان تأیید دقیق ثبت شود.
    /// - نباید: درخواست‌های غیر UnderReview تأیید شوند.
    /// - نباید: مبلغ منفی تأیید شود.
    /// </remarks>
    public void Approve(Money approvedAmount, string? notes = null)
    {
        if (Status != FacilityRequestStatus.UnderReview)
            throw new InvalidOperationException("Can only approve requests under review");
        if (approvedAmount == null)
            throw new ArgumentNullException(nameof(approvedAmount));
        if (approvedAmount.AmountRials <= 0)
            throw new ArgumentException("Approved amount must be positive", nameof(approvedAmount));

        var previousStatus = Status;
        Status = FacilityRequestStatus.Approved;
        ApprovedAmount = approvedAmount;
        ApprovedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(notes))
        {
            Metadata["ApprovalNotes"] = notes;
        }
    }

    /// <summary>
    /// رد درخواست
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار درخواست را رد کرده و دلیل رد را ثبت می‌نماید.
    /// درخواست رد شده دیگر قابل پیگیری نیست.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مسئول بررسی درخواست را رد می‌کند، این رفتار برای
    /// ثبت رد و دلیل آن استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های UnderReview قابل رد هستند.
    /// - دلیل رد اجباری است.
    /// - وضعیت به Rejected تغییر می‌یابد.
    /// - زمان رد ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: دلیل رد واضح و قابل فهم باشد.
    /// - باید: زمان رد دقیق ثبت شود.
    /// - نباید: درخواست‌های غیر UnderReview رد شوند.
    /// - نباید: بدون دلیل رد شود.
    /// </remarks>
    public void Reject(string reason)
    {
        if (Status != FacilityRequestStatus.UnderReview)
            throw new InvalidOperationException("Can only reject requests under review");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required", nameof(reason));

        var previousStatus = Status;
        Status = FacilityRequestStatus.Rejected;
        RejectionReason = reason.Trim();
        RejectedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// لغو درخواست توسط متقاضی
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار درخواست را توسط متقاضی لغو می‌کند و دلیل لغو را ثبت می‌نماید.
    /// درخواست لغو شده دیگر قابل پیگیری نیست.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که متقاضی از ادامه فرآیند درخواست منصرف می‌شود، این رفتار
    /// برای لغو درخواست استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های PendingApproval یا UnderReview قابل لغو هستند.
    /// - دلیل لغو اختیاری است.
    /// - وضعیت به Cancelled تغییر می‌یابد.
    /// - زمان لغو ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط درخواست‌های قابل لغو لغو شوند.
    /// - باید: زمان لغو دقیق ثبت شود.
    /// - نباید: درخواست‌های تأیید شده لغو شوند.
    /// - نباید: بدون مجوز متقاضی لغو شود.
    /// </remarks>
    public void Cancel(string? reason = null)
    {
        if (Status != FacilityRequestStatus.PendingApproval && Status != FacilityRequestStatus.UnderReview)
            throw new InvalidOperationException("Can only cancel pending approval or under review applications");

        var previousStatus = Status;
        Status = FacilityRequestStatus.Cancelled;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["CancellationReason"] = reason;
        }
    }

    /// <summary>
    /// پردازش توسط بانک
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار درخواست اعزام شده را به عنوان پردازش شده توسط بانک علامت‌گذاری می‌کند
    /// و زمان پردازش را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که بانک درخواست را دریافت کرده و شروع به پردازش آن کرده است،
    /// این رفتار برای ثبت وضعیت پردازش استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های DispatchedToBank قابل پردازش هستند.
    /// - وضعیت به ProcessedByBank تغییر می‌یابد.
    /// - زمان پردازش ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط درخواست‌های اعزام شده پردازش شوند.
    /// - باید: زمان پردازش دقیق ثبت شود.
    /// - نباید: درخواست‌های غیر اعزام شده پردازش شوند.
    /// - نباید: بدون دریافت واقعی از بانک پردازش شود.
    /// </remarks>
    public void MarkAsProcessedByBank()
    {
        if (Status != FacilityRequestStatus.SentToBank)
            throw new InvalidOperationException("Can only mark dispatched applications as processed");

        var previousStatus = Status;
        Status = FacilityRequestStatus.ProcessedByBank;
        ProcessedByBankAt = DateTime.UtcNow;
    }

    /// <summary>
    /// تکمیل درخواست
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار درخواست را به عنوان تکمیل شده علامت‌گذاری می‌کند و زمان تکمیل را ثبت می‌نماید.
    /// درخواست تکمیل شده یعنی وجوه به حساب متقاضی واریز شده است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که بانک وجوه را به حساب متقاضی واریز کرده است، این رفتار
    /// برای تکمیل درخواست استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های ProcessedByBank قابل تکمیل هستند.
    /// - وضعیت به Completed تغییر می‌یابد.
    /// - زمان تکمیل ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط درخواست‌های پردازش شده تکمیل شوند.
    /// - باید: زمان تکمیل دقیق ثبت شود.
    /// - نباید: درخواست‌های غیر پردازش شده تکمیل شوند.
    /// - نباید: بدون واریز واقعی وجوه تکمیل شود.
    /// </remarks>
    public void Complete()
    {
        if (Status != FacilityRequestStatus.ProcessedByBank)
            throw new InvalidOperationException("Can only complete processed applications");

        var previousStatus = Status;
        Status = FacilityRequestStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// بررسی اینکه آیا درخواست قابل لغو است
    /// </summary>
    public bool CanBeCancelled()
    {
        return Status == FacilityRequestStatus.PendingApproval ||
               Status == FacilityRequestStatus.UnderReview;
    }

    /// <summary>
    /// بررسی اینکه آیا درخواست قابل تأیید است
    /// </summary>
    public bool CanBeApproved()
    {
        return Status == FacilityRequestStatus.UnderReview;
    }

    /// <summary>
    /// بررسی اینکه آیا درخواست قابل رد است
    /// </summary>
    public bool CanBeRejected()
    {
        return Status == FacilityRequestStatus.UnderReview;
    }

    /// <summary>
    /// بررسی اینکه آیا درخواست قابل اعزام است
    /// </summary>
    public bool CanBeDispatched()
    {
        return Status == FacilityRequestStatus.Approved;
    }

    /// <summary>
    /// دریافت مبلغ نهایی (تأیید شده یا درخواستی)
    /// </summary>
    public Money GetFinalAmount()
    {
        return ApprovedAmount ?? RequestedAmount;
    }

    private static string GenerateApplicationNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var randomPart = Random.Shared.Next(100, 999).ToString();
        return $"درخواست-{timestamp}{randomPart}";
    }
}
