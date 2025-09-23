using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// واریز کیف پول - درخواست واریز وجه به کیف پول کاربر
/// در دنیای واقعی: درخواست کاربر برای افزایش موجودی کیف پول از طریق پرداخت
/// </summary>
public sealed class WalletDeposit : FullAggregateRoot<Guid>
{
    public Guid WalletId { get; private set; }
    public Guid? BillId { get; private set; }
    public string UserNationalNumber { get; private set; } = null!;
    public Money Amount { get; private set; } = null!;
    public WalletDepositStatus Status { get; private set; }
    public string? Description { get; private set; }
    public string? ExternalReference { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    public Wallet Wallet { get; private set; } = null!;
    public Bill? Bill { get; private set; }

    // Private constructor for EF Core
    private WalletDeposit() : base() { }

    /// <summary>
    /// ایجاد درخواست واریز جدید
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک درخواست واریز جدید برای کیف پول کاربر ایجاد می‌کند که شامل
    /// مبلغ واریز، توضیحات و سایر اطلاعات مرتبط است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر می‌خواهد موجودی کیف پول خود را افزایش دهد، این رفتار برای
    /// ایجاد درخواست واریز و پیگیری فرآیند پرداخت استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه کیف پول اجباری و معتبر باشد.
    /// - مبلغ واریز باید مثبت و معتبر باشد.
    /// - وضعیت اولیه همیشه در انتظار خواهد بود.
    /// - زمان درخواست به صورت خودکار ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه کیف پول و مبلغ معتبر ارائه شود.
    /// - باید: مبلغ مثبت باشد.
    /// - نباید: درخواست بدون شناسه کیف پول ایجاد شود.
    /// - نباید: مبلغ صفر یا منفی پذیرفته شود.
    /// </remarks>
    public WalletDeposit(
        Guid walletId,
        string userNationalNumber,
        Money amount,
        string? description = null,
        string? externalReference = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (walletId == Guid.Empty)
            throw new ArgumentException("Wallet ID cannot be empty", nameof(walletId));
        if (string.IsNullOrWhiteSpace(userNationalNumber))
            throw new ArgumentException("User national number cannot be empty", nameof(userNationalNumber));
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Deposit amount must be positive", nameof(amount));

        WalletId = walletId;
        UserNationalNumber = userNationalNumber.Trim();
        Amount = amount;
        Status = WalletDepositStatus.Pending;
        Description = description?.Trim();
        ExternalReference = externalReference?.Trim();
        RequestedAt = DateTime.UtcNow;
        Metadata = metadata ?? new Dictionary<string, string>();

        // Raise domain event
        AddDomainEvent(new WalletDepositRequestedEvent(
            Id,
            WalletId,
            UserNationalNumber,
            Amount,
            RequestedAt,
            Description));
    }

    /// <summary>
    /// تکمیل واریز
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار واریز را به عنوان تکمیل شده علامت‌گذاری کرده و زمان تکمیل را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که پرداخت مربوط به واریز موفقیت‌آمیز باشد، این رفتار برای تکمیل فرآیند
    /// واریز و آماده‌سازی برای اعمال به کیف پول استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - واریز باید در وضعیت در انتظار باشد.
    /// - زمان تکمیل به صورت خودکار ثبت می‌شود.
    /// - وضعیت به تکمیل شده تغییر می‌کند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: واریز در وضعیت در انتظار باشد.
    /// - باید: زمان تکمیل ثبت شود.
    /// - نباید: واریز‌های تکمیل شده مجدداً تکمیل شوند.
    /// </remarks>
    public void Complete()
    {
        if (Status != WalletDepositStatus.Pending)
            throw new InvalidOperationException("Can only complete pending deposits");

        Status = WalletDepositStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        // Raise domain event
        AddDomainEvent(new WalletDepositCompletedEvent(
            Id,
            WalletId,
            UserNationalNumber,
            Amount,
            CompletedAt.Value,
            Description));
    }

    /// <summary>
    /// لغو واریز
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار واریز را لغو کرده و دلیل لغو را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که پرداخت ناموفق باشد یا کاربر درخواست لغو دهد، این رفتار برای
    /// لغو واریز و جلوگیری از اعمال آن به کیف پول استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - واریز باید در وضعیت در انتظار باشد.
    /// - وضعیت به لغو شده تغییر می‌کند.
    /// - زمان تکمیل به زمان لغو تغییر می‌کند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: واریز در وضعیت در انتظار باشد.
    /// - باید: دلیل لغو مشخص باشد.
    /// - نباید: واریز‌های تکمیل شده لغو شوند.
    /// </remarks>
    public void Cancel(string? reason = null)
    {
        if (Status != WalletDepositStatus.Pending)
            throw new InvalidOperationException("Can only cancel pending deposits");

        Status = WalletDepositStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(reason))
        {
            Metadata["CancellationReason"] = reason;
        }

        // Raise domain event
        AddDomainEvent(new WalletDepositCancelledEvent(
            Id,
            WalletId,
            UserNationalNumber,
            Amount,
            CompletedAt.Value,
            reason));
    }

    /// <summary>
    /// افزودن اطلاعات اضافی به واریز
    /// </summary>
    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be empty", nameof(key));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Metadata value cannot be empty", nameof(value));

        Metadata[key.Trim()] = value.Trim();
    }

    /// <summary>
    /// Set the bill ID for this deposit
    /// </summary>
    public void SetBillId(Guid billId)
    {
        if (billId == Guid.Empty)
            throw new ArgumentException("Bill ID cannot be empty", nameof(billId));

        BillId = billId;
    }
}
