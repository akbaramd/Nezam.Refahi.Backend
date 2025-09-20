using MCA.SharedKernel.Domain;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// برگشت وجه - نماینده درخواست و فرآیند بازگشت پول
/// در دنیای واقعی: هر عملیات بازگشت پول به دلیل عودت کالا، لغو سفارش یا عدم رضایت
/// </summary>
public sealed class Refund : Entity<Guid>
{
    public Guid BillId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public RefundStatus Status { get; private set; }
    public string Reason { get; private set; } = null!;
    public string RequestedByNationalNumber { get; private set; } = null!;
    public DateTime RequestedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? GatewayRefundId { get; private set; }
    public string? GatewayReference { get; private set; }
    public string? ProcessorNotes { get; private set; }
    public string? RejectionReason { get; private set; }

    // Navigation property
    public Bill Bill { get; private set; } = null!;

    // Private constructor for EF Core
    private Refund() : base() { }

    /// <summary>
    /// ایجاد درخواست برگشت وجه جدید
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک درخواست برگشت وجه جدید برای صورت حساب پرداخت شده ایجاد می‌کند
    /// که شامل مبلغ، دلیل و شناسه درخواست‌کننده است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مشتری به دلیل عودت کالا، لغو سفارش یا عدم رضایت درخواست بازگشت پول می‌کند،
    /// این رفتار برای شروع فرآیند برگشت وجه استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه صورت حساب اجباری و معتبر باشد.
    /// - مبلغ برگشت باید مثبت و معتبر باشد.
    /// - دلیل برگشت اجباری و واضح باشد.
    /// - شناسه ملی درخواست‌کننده ضروری است.
    /// - وضعیت اولیه همیشه در انتظار خواهد بود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: اطلاعات ضروری برای بررسی تکمیل باشد.
    /// - باید: دلیل برگشت قابل فهم و معتبر باشد.
    /// - نباید: بدون شناسه صورت حساب ایجاد شود.
    /// - نباید: مبلغ صفر یا منفی پذیرفته شود.
    /// </remarks>
    public Refund(
        Guid billId,
        Money amount,
        string reason,
        string requestedByNationalNumber)
        : base(Guid.NewGuid())
    {
        if (billId == Guid.Empty)
            throw new ArgumentException("Bill ID cannot be empty", nameof(billId));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Refund reason cannot be empty", nameof(reason));
        if (string.IsNullOrWhiteSpace(requestedByNationalNumber))
            throw new ArgumentException("Requester national number cannot be empty", nameof(requestedByNationalNumber));

        BillId = billId;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Reason = reason.Trim();
        RequestedByNationalNumber = requestedByNationalNumber.Trim();
        Status = RefundStatus.Pending;
        RequestedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// تایید درخواست برگشت وجه و شروع پردازش
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار درخواست برگشت وجه را تایید کرده و وضعیت آن را به حال پردازش تغییر می‌دهد
    /// و در صورت وجود یادداشت پردازش‌کننده آن را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که خزانهدار یا مدیر مالی درخواست برگشت وجه را بررسی و تایید می‌کند،
    /// این رفتار برای شروع فرآیند برگشت پول استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های در حالت انتظار قابل تایید هستند.
    /// - وضعیت باید به حال پردازش تغییر کند.
    /// - زمان پردازش به صورت خودکار ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت انتظار قابل تایید باشد.
    /// - باید: زمان پردازش دقیق ثبت شود.
    /// - نباید: درخواست‌های تکمیل شده قابل تایید باشند.
    /// - نباید: درخواست‌های رد شده قابل تغییر باشند.
    /// </remarks>
    public void Approve(string? processorNotes = null)
    {
        if (Status != RefundStatus.Pending)
            throw new InvalidOperationException("Can only approve pending refunds");

        Status = RefundStatus.Processing;
        ProcessedAt = DateTime.UtcNow;
        ProcessorNotes = processorNotes?.Trim();
    }

    /// <summary>
    /// رد درخواست برگشت وجه
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار درخواست برگشت وجه را رد کرده و دلیل رد را ثبت می‌نماید
    /// تا درخواست‌کننده از علت عدم تایید آگاه شود.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مدیر یا خزانهدار پس از بررسی درخواست، به دلایلی چون عدم تطابق با قوانین
    /// یا عدم ارائه مدارک کافی، درخواست برگشت وجه را رد می‌کند.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های در حالت انتظار قابل رد هستند.
    /// - دلیل رد اجباری و باید واضح باشد.
    /// - وضعیت باید به رد شده تغییر کند.
    /// - زمان پردازش به صورت خودکار ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: دلیل رد واضح و قابل فهم ارائه شود.
    /// - باید: زمان رد دقیق ثبت شود.
    /// - نباید: درخواست‌های تایید شده قابل رد باشند.
    /// - نباید: بدون دلیل واضح رد شود.
    /// </remarks>
    public void Reject(string rejectionReason)
    {
        if (Status != RefundStatus.Pending)
            throw new InvalidOperationException("Can only reject pending refunds");
        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("Rejection reason is required", nameof(rejectionReason));

        Status = RefundStatus.Rejected;
        RejectionReason = rejectionReason.Trim();
        ProcessedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// علامت‌گذاری برگشت وجه به عنوان تکمیل شده
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار برگشت وجه را به عنوان تکمیل شده علامت‌گذاری کرده و اطلاعات نهایی درگاه
    /// و زمان تکمیل را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که خزانهدار پس از تایید درخواست، فرآیند بازگشت پول را به حساب مشتری
    /// تکمیل می‌کند، این رفتار برای ثبت نهایی بازگشت وجه استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های در حال پردازش قابل تکمیل هستند.
    /// - وضعیت باید به تکمیل شده تغییر کند.
    /// - زمان تکمیل به صورت خودکار ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حال پردازش قابل تکمیل باشد.
    /// - باید: زمان تکمیل دقیق ثبت شود.
    /// - نباید: درخواست‌های رد شده قابل تکمیل باشند.
    /// - نباید: درخواست‌های قبلاً تکمیل شده تغییر کنند.
    /// </remarks>
    public void MarkAsCompleted(string? gatewayRefundId = null, string? gatewayReference = null)
    {
        if (Status != RefundStatus.Processing)
            throw new InvalidOperationException("Can only complete processing refunds");

        Status = RefundStatus.Completed;
        GatewayRefundId = gatewayRefundId?.Trim();
        GatewayReference = gatewayReference?.Trim();
        CompletedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// علامت‌گذاری برگشت وجه به عنوان ناموفق
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار برگشت وجه را به عنوان ناموفق علامت‌گذاری کرده و دلیل شکست
    /// را برای پیگیری و بررسی مجدد ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که به دلیل مشکل فنی در درگاه، عدم موجودی کافی یا مسائل اداری
    /// امکان بازگشت پول وجود ندارد، این رفتار برای ثبت شکست استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط درخواست‌های در حال پردازش قابل تغییر به ناموفق هستند.
    /// - وضعیت باید به ناموفق تغییر کند.
    /// - دلیل شکست برای بررسی آینده لازم است.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حال پردازش قابل تغییر باشد.
    /// - باید: دلیل شکست واضح و قابل فهم باشد.
    /// - نباید: درخواست‌های تکمیل شده قابل تغییر باشند.
    /// - نباید: بدون دلیل معتبر ناموفق شود.
    /// </remarks>
    public void MarkAsFailed(string? failureReason = null)
    {
        if (Status != RefundStatus.Processing)
            throw new InvalidOperationException("Can only fail processing refunds");

        Status = RefundStatus.Failed;
        ProcessorNotes = failureReason?.Trim();
    }

}