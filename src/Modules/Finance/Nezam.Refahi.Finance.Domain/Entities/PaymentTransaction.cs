using MCA.SharedKernel.Domain;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// تراکنش پرداخت - ثبت جزئیات هر تلاش یا گزارش پردازش پرداخت
/// </summary>
public sealed class PaymentTransaction : Entity<Guid>
{
    public Guid PaymentId { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? GatewayReference { get; private set; }
    public string? GatewayResponse { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Money? ProcessedAmount { get; private set; }

    // Navigation property
    public Payment Payment { get; private set; } = null!;

    // Private constructor for EF Core
    private PaymentTransaction() : base() { }

    /// <summary>
    /// ایجاد تراکنش پرداخت جدید برای ثبت جزئیات پردازش
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک رکورد جدید از تلاش پردازش پرداخت ایجاد می‌کند که شامل وضعیت، شناسه‌های درگاه
    /// و سایر اطلاعات فنی مربوط به هر مرحله از پردازش پرداخت است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که درگاه پرداخت هر تلاشی برای پردازش تراکنش انجام می‌دهد، این رفتار اجرا شده
    /// و جزئیات کامل آن تلاش برای پیگیری و عیب‌یابی ثبت می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه پرداخت مرتبط اجباری است.
    /// - وضعیت اولیه تراکنش باید تعیین شود.
    /// - زمان ایجاد به صورت خودکار ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه پرداخت معتبر ارائه شود.
    /// - باید: وضعیت تراکنش مطابق با مرحله پردازش باشد.
    /// - نباید: شناسه پرداخت خالی یا نامعتبر باشد.
    /// </remarks>
    public PaymentTransaction(
        Guid paymentId,
        PaymentStatus status,
        string? gatewayTransactionId = null,
        string? gatewayReference = null,
        string? gatewayResponse = null,
        Money? processedAmount = null)
        : base(Guid.NewGuid())
    {
        if (paymentId == Guid.Empty)
            throw new ArgumentException("Payment ID cannot be empty", nameof(paymentId));

        PaymentId = paymentId;
        Status = status;
        GatewayTransactionId = gatewayTransactionId?.Trim();
        GatewayReference = gatewayReference?.Trim();
        GatewayResponse = gatewayResponse?.Trim();
        ProcessedAmount = processedAmount;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// ثبت اطلاعات خطا برای تراکنش ناموفق
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار اطلاعات خطای دریافتی از درگاه یا سیستم پردازش را ثبت کرده و وضعیت تراکنش
    /// را به ناموفق تغییر می‌دهد تا دلیل شکست برای بررسی و حل مشکل قابل دسترس باشد.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که درگاه پرداخت خطا برگرداند، موجودی کافی نباشد یا مشکل فنی رخ دهد،
    /// این رفتار برای ثبت دقیق علت شکست و ارائه بازخورد مناسب به کاربر استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کد خطا و پیام خطا برای درک بهتر مشکل ضروری است.
    /// - وضعیت تراکنش باید به ناموفق تغییر کند.
    /// - اطلاعات خطا باید برای گزارش‌گیری محفوظ بماند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کد خطا و پیام خطای واضح ثبت شود.
    /// - باید: وضعیت به ناموفق تغییر کند.
    /// - نباید: اطلاعات حساس در پیام خطا افشا شود.
    /// </remarks>
    public void SetError(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode?.Trim();
        ErrorMessage = errorMessage?.Trim();
        Status = PaymentStatus.Failed;
    }

    /// <summary>
    /// بروزرسانی پاسخ دریافتی از درگاه پرداخت
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار پاسخ کامل دریافتی از درگاه پرداخت را در سیستم ثبت می‌کند تا برای
    /// عیب‌یابی، گزارش‌گیری و پیگیری مسائل فنی در دسترس باشد.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که درگاه پرداخت پاسخ کاملی از نتیجه پردازش ارسال می‌کند، این رفتار
    /// برای ذخیره آن پاسخ جهت بررسی‌های آینده و حل مسائل احتمالی استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - پاسخ درگاه باید کامل و قابل خواندن باشد.
    /// - اطلاعات ثبت شده نباید دستکاری شود.
    /// - تاریخ بروزرسانی به صورت ضمنی ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: پاسخ کامل درگاه محفوظ شود.
    /// - باید: فرمت پاسخ حفظ گردد.
    /// - نباید: اطلاعات حساس در لاگ افشا شود.
    /// </remarks>
    public void UpdateGatewayResponse(string gatewayResponse)
    {
        GatewayResponse = gatewayResponse?.Trim();
    }
}