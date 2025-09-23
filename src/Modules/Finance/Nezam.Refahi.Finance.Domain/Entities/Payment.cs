using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// پرداخت - نماینده تلاش پرداخت برای یک صورت حساب
/// در دنیای واقعی: هر عملیات پرداختی چه آنلاین، کارتخوان یا نقدی
/// </summary>
public sealed class Payment : SoftDeletableAggregateRoot<Guid>
{
    public Guid BillId { get; private set; }
    public string BillNumber { get; private set; } = null!;
    public Money Amount { get; private set; } = null!;
    public string? TrackingNumber { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentGateway? Gateway { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? GatewayReference { get; private set; }
    public string? CallbackUrl { get; private set; }
    public string? Description { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }

    // Navigation properties
    public Bill Bill { get; private set; } = null!;

    private readonly List<PaymentTransaction> _transactions = new();
    public IReadOnlyCollection<PaymentTransaction> Transactions => _transactions.AsReadOnly();

    // Private constructor for EF Core
    private Payment() : base() { }

    /// <summary>
    /// ایجاد پرداخت جدید برای صورت حساب
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک تلاش پرداخت جدید برای صورت حساب ایجاد می‌کند که شامل مبلغ، روش پرداخت،
    /// درگاه انتخابی و تاریخ انقضا است و در حالت انتظار قرار می‌گیرد.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مشتری در فروشگاه آنلاین یا دفتر خدماتی تصمیم به پرداخت می‌گیرد،
    /// این رفتار اجرا شده و جزئیات پرداخت برای پیگیری ثبت می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه صورت حساب و شماره سند اجباری است.
    /// - مبلغ پرداخت باید معتبر و مثبت باشد.
    /// - تاریخ انقضا پیش‌فرض 15 دقیقه پس از ایجاد است.
    /// - وضعیت اولیه همیشه در انتظار خواهد بود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه صورت حساب معتبر ارائه شود.
    /// - باید: مبلغ مثبت و قابل پردازش باشد.
    /// - نباید: تاریخ انقضا در گذشته تعیین شود.
    /// - نباید: بدون شناسه صورت حساب ایجاد شود.
    /// </remarks>
    public Payment(
        Guid billId,
        string billNumber,
        Money amount,
        PaymentMethod method = PaymentMethod.Online,
        PaymentGateway? gateway = null,
        string? callbackUrl = null,
        string? description = null,
        DateTime? expiryDate = null)
        : base(Guid.NewGuid())
    {
        if (billId == Guid.Empty)
            throw new ArgumentException("Bill ID cannot be empty", nameof(billId));
        if (string.IsNullOrWhiteSpace(billNumber))
            throw new ArgumentException("Bill number cannot be empty", nameof(billNumber));

        BillId = billId;
        BillNumber = billNumber.Trim();
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Method = method;
        Gateway = gateway;
        CallbackUrl = callbackUrl?.Trim();
        Description = description?.Trim();
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        ExpiryDate = expiryDate ?? DateTime.UtcNow.AddMinutes(15); // Default 15 minutes
    }

    /// <summary>
    /// علامت‌گذاری پرداخت در حال پردازش
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار وضعیت پرداخت را از انتظار به حال پردازش تغییر می‌دهد و در صورت وجود
    /// شناسه تراکنش از درگاه، آن را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که درگاه پرداخت تراکنش را دریافت کرده و شروع به بررسی آن کرده است،
    /// این رفتار برای اطلاع‌رسانی وضعیت پردازش به کاربر استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط پرداخت‌های در حالت انتظار قابل تغییر به حال پردازش هستند.
    /// - شناسه تراکنش درگاه در صورت وجود ثبت می‌شود.
    /// - وضعیت پرداخت باید به پردازش تغییر کند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط از حالت انتظار قابل تغییر باشد.
    /// - باید: شناسه تراکنش معتبر در صورت وجود ثبت شود.
    /// - نباید: پرداخت‌های تکمیل شده قابل تغییر باشند.
    /// </remarks>
    public void SetProcessing(string? gatewayTransactionId = null)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Can only set processing status for pending payments");

        Status = PaymentStatus.Processing;
        GatewayTransactionId = gatewayTransactionId?.Trim();

        // Raise domain event
        AddDomainEvent(new PaymentProcessingEvent(
            Id,
            BillId,
            BillNumber,
            Bill.ReferenceId,
            Bill.UserNationalNumber,
            Amount,
            Gateway!.Value,
            gatewayTransactionId,
            TrackingNumber,
            CallbackUrl));
    }

    /// <summary>
    /// علامت‌گذاری پرداخت به عنوان موفق
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار پرداخت را به عنوان موفق علامت‌گذاری کرده، شناسه تراکنش نهایی و مرجع درگاه
    /// را ثبت می‌کند و زمان تکمیل را تنظیم می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که درگاه پرداخت تایید نهایی دریافت پول را اعلام کند یا خزانهدار
    /// دریافت نقدی را تصدیق نماید، این رفتار اجرا می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - فقط پرداخت‌های در حال پردازش یا انتظار قابل تکمیل هستند.
    /// - شناسه تراکنش درگاه اجباری است.
    /// - زمان تکمیل به صورت خودکار ثبت می‌شود.
    /// - تاریخ انقضا پاک می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه تراکنش معتبر ارائه شود.
    /// - باید: زمان تکمیل دقیق ثبت شود.
    /// - نباید: پرداخت‌های ناموفق قابل تکمیل باشند.
    /// - نباید: بدون شناسه تراکنش تکمیل شود.
    /// </remarks>
    public void MarkAsCompleted(string gatewayTransactionId, string? gatewayReference = null)
    {
        if (Status != PaymentStatus.Processing && Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Can only complete processing or pending payments");

        Status = PaymentStatus.Completed;
        GatewayTransactionId = gatewayTransactionId?.Trim();
        GatewayReference = gatewayReference?.Trim();
        CompletedAt = DateTime.UtcNow;
        ExpiryDate = null; // Clear expiry on completion
    }

    public void SetTrackingNumber(string? trackingNumber)
    {
        TrackingNumber = trackingNumber;
    }
    

    /// <summary>
    /// علامت‌گذاری پرداخت به عنوان ناموفق
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار پرداخت را به عنوان ناموفق علامت‌گذاری کرده و دلیل شکست را
    /// برای پیگیری و ارائه بازخورد مناسب به کاربر ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که درگاه پرداخت خطا برگرداند، موجودی کافی نباشد یا بانک تراکنش را رد کند،
    /// این رفتار برای ثبت دقیق علت شکست استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - پرداخت‌های موفق قابل تغییر به ناموفق نیستند.
    /// - دلیل شکست برای عیب‌یابی ضروری است.
    /// - وضعیت پرداخت باید به ناموفق تغییر کند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: دلیل واضح و قابل فهم شکست ثبت شود.
    /// - باید: وضعیت به ناموفق تغییر کند.
    /// - نباید: پرداخت‌های موفق تغییر داده شوند.
    /// - نباید: اطلاعات حساس در دلیل شکست افشا شود.
    /// </remarks>
    public void MarkAsFailed(string? failureReason = null)
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot mark completed payment as failed");

        Status = PaymentStatus.Failed;
        FailureReason = failureReason?.Trim();
    }

    /// <summary>
    /// لغو پرداخت
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار پرداخت را به عنوان لغو شده علامت‌گذاری کرده و دلیل لغو
    /// را در صورت وجود ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر از ادامه فرآیند پرداخت منصرف شود یا مدیر سیستم
    /// برای دلایل مختلف پرداخت را لغو کند، این رفتار استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - پرداخت‌های موفق قابل لغو نیستند.
    /// - پرداخت‌های قبلاً لغو شده تغییری نمی‌کنند.
    /// - دلیل لغو می‌تواند اختیاری باشد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: وضعیت به لغو شده تغییر کند.
    /// - باید: دلیل لغو در صورت وجود ثبت شود.
    /// - نباید: پرداخت‌های تکمیل شده لغو شوند.
    /// - نباید: عملیات تکراری لغو انجام شود.
    /// </remarks>
    public void Cancel(string? reason = null)
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed payment");
        if (Status == PaymentStatus.Cancelled)
            return; // Already cancelled

        Status = PaymentStatus.Cancelled;
        FailureReason = reason?.Trim();
    }

    /// <summary>
    /// علامت‌گذاری پرداخت به عنوان منقضی
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار پرداخت را به عنوان منقضی علامت‌گذاری می‌کند
    /// زمانی که زمان مجاز پرداخت سپری شده و عملیات تکمیل نشده باشد.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که مهلت تعیین شده برای تکمیل پرداخت گذشته و کاربر عملیات را تکمیل نکرده،
    /// این رفتار برای پاکسازی زمانبندی و ازادسازی منابع استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - پرداخت‌های موفق قابل انقضا نیستند.
    /// - فقط پرداخت‌های در حالت انتظار قابل انقضا هستند.
    /// - وضعیت باید به منقضی تغییر کند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط پرداخت‌های در انتظار قابل انقضا باشند.
    /// - باید: وضعیت به منقضی تغییر کند.
    /// - نباید: پرداخت‌های موفق انقضا یابند.
    /// - نباید: پرداخت‌های غیر انتظار تغییر داده شوند.
    /// </remarks>
    public void MarkAsExpired()
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot expire completed payment");
        if (Status != PaymentStatus.Pending)
            return; // Only pending payments can expire

        Status = PaymentStatus.Expired;

        // Raise domain event
        AddDomainEvent(new PaymentExpiredEvent(
            Id,
            BillId,
            BillNumber,
            Bill.ReferenceId,
            Bill.UserNationalNumber,
            Amount,
            Method,
            Gateway,
            TrackingNumber,
            ExpiryDate));
    }

    /// <summary>
    /// بررسی منقضی بودن پرداخت
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار بررسی می‌کند که آیا زمان مجاز پرداخت سپری شده و پرداخت
    /// هنوز در حالت انتظار قرار دارد یا خیر.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که سیستم به صورت خودکار پرداخت‌های قدیمی و منقضی را بررسی و پاکسازی می‌کند
    /// یا زمانی که نیاز به بررسی وضعیت پرداخت وجود دارد.
    ///
    /// <para>قوانین:</para>
    /// - فقط پرداخت‌های در حالت انتظار قابل بررسی هستند.
    /// - تاریخ انقضا باید تعیین شده باشد.
    /// - مقایسه با زمان جاری بر اساس UTC انجام می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: فقط پرداخت‌های در انتظار بررسی شوند.
    /// - باید: تاریخ انقضا معتبر و تعیین شده باشد.
    /// - نباید: پرداخت‌های غیر انتظار بررسی شوند.
    /// - نباید: بدون تاریخ انقضا معتبر بررسی شود.
    /// </remarks>
    public bool IsExpired()
    {
        return Status == PaymentStatus.Pending &&
               ExpiryDate.HasValue &&
               DateTime.UtcNow > ExpiryDate.Value;
    }
}