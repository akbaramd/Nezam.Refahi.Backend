using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// کیف پول - مدیریت موجودی و تراکنش‌های مالی کاربران
/// در دنیای واقعی: حساب دیجیتال کاربر برای ذخیره، انتقال و پرداخت وجه
/// </summary>
public sealed class Wallet : FullAggregateRoot<Guid>
{
    public string NationalNumber { get; private set; } = null!;
    public Money Balance { get; private set; } = null!;
    public WalletStatus Status { get; private set; }
    public string? WalletName { get; private set; }
    public string? Description { get; private set; }
    public DateTime? LastTransactionAt { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    private readonly List<WalletTransaction> _transactions = new();
    public IReadOnlyCollection<WalletTransaction> Transactions => _transactions.AsReadOnly();

    // Private constructor for EF Core
    private Wallet() : base() { }

    /// <summary>
    /// ایجاد کیف پول جدید برای کاربر
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک کیف پول جدید برای کاربر با کد ملی مشخص ایجاد می‌کند
    /// که شامل موجودی اولیه، نام کیف پول و سایر تنظیمات اولیه است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر جدید در سیستم ثبت‌نام می‌کند یا نیاز به ایجاد کیف پول
    /// برای مدیریت مالی شخصی دارد، این رفتار استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کد ملی کاربر اجباری و معتبر باشد.
    /// - موجودی اولیه معمولاً صفر است.
    /// - وضعیت اولیه همیشه فعال خواهد بود.
    /// - نام کیف پول اختیاری است.
    /// - زمان ایجاد به صورت خودکار ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کد ملی معتبر ارائه شود.
    /// - باید: موجودی اولیه معتبر باشد.
    /// - نباید: کیف پول بدون کد ملی ایجاد شود.
    /// - نباید: موجودی اولیه منفی باشد.
    /// </remarks>
    public Wallet(
        string nationalNumber,
        Money? initialBalance = null,
        string? walletName = null,
        string? description = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (string.IsNullOrWhiteSpace(nationalNumber))
            throw new ArgumentException("National number cannot be empty", nameof(nationalNumber));

        NationalNumber = nationalNumber.Trim();
        Balance = initialBalance ?? Money.Zero;
        Status = WalletStatus.Active;
        WalletName = walletName?.Trim();
        Description = description?.Trim();
        CreatedAt = DateTime.UtcNow;
        Metadata = metadata ?? new Dictionary<string, string>();

        // Raise domain event
        AddDomainEvent(new WalletCreatedEvent(
            Id,
            NationalNumber,
            Balance,
            CreatedAt));
    }

    /// <summary>
    /// واریز وجه به کیف پول
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار مبلغ مشخصی را به کیف پول واریز کرده و موجودی را افزایش می‌دهد
    /// و تراکنش مربوطه را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر از طریق درگاه بانکی، کارتخوان یا سایر روش‌ها وجه واریز می‌کند،
    /// این رفتار برای افزایش موجودی و ثبت تراکنش استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کیف پول باید فعال باشد.
    /// - مبلغ واریز باید مثبت باشد.
    /// - موجودی جدید محاسبه و ثبت می‌شود.
    /// - تراکنش واریز ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کیف پول فعال باشد.
    /// - باید: مبلغ مثبت باشد.
    /// - نباید: به کیف پول غیرفعال واریز شود.
    /// - نباید: مبلغ صفر یا منفی واریز شود.
    /// </remarks>
    public WalletTransaction Deposit(
        Money amount,
        string? referenceId = null,
        string? description = null,
        string? externalReference = null,
        Dictionary<string, string>? metadata = null)
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Cannot deposit to inactive wallet");
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Deposit amount must be positive", nameof(amount));

        var previousBalance = Balance;
        Balance = Balance.Add(amount);
        LastTransactionAt = DateTime.UtcNow;

        var transaction = new WalletTransaction(
            Id,
            WalletTransactionType.Deposit,
            amount,
            Balance,
            referenceId,
            description,
            externalReference,
            metadata);

        _transactions.Add(transaction);

        // Raise domain events
        AddDomainEvent(new WalletBalanceChangedEvent(
            Id,
            NationalNumber,
            WalletTransactionType.Deposit,
            previousBalance,
            Balance,
            amount,
            referenceId,
            DateTime.UtcNow));

        AddDomainEvent(new WalletTransactionCompletedEvent(
            transaction.Id,
            Id,
            NationalNumber,
            WalletTransactionType.Deposit,
            amount,
            Balance,
            referenceId,
            description,
            DateTime.UtcNow));

        return transaction;
    }

    /// <summary>
    /// برداشت وجه از کیف پول
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار مبلغ مشخصی را از کیف پول برداشت کرده و موجودی را کاهش می‌دهد
    /// و تراکنش مربوطه را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر نیاز به برداشت وجه از کیف پول دارد یا برای پرداخت فاکتور
    /// از موجودی کیف پول استفاده می‌کند، این رفتار اجرا می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کیف پول باید فعال باشد.
    /// - موجودی کافی باید وجود داشته باشد.
    /// - مبلغ برداشت باید مثبت باشد.
    /// - موجودی جدید محاسبه و ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: موجودی کافی وجود داشته باشد.
    /// - باید: کیف پول فعال باشد.
    /// - نباید: بیش از موجودی برداشت شود.
    /// - نباید: از کیف پول غیرفعال برداشت شود.
    /// </remarks>
    public WalletTransaction Withdraw(
        Money amount,
        string? referenceId = null,
        string? description = null,
        string? externalReference = null)
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Cannot withdraw from inactive wallet");
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Withdrawal amount must be positive", nameof(amount));
        if (Balance.IsLessThan(amount))
            throw new InvalidOperationException("Insufficient balance");

        var previousBalance = Balance;
        Balance = Balance.Subtract(amount);
        LastTransactionAt = DateTime.UtcNow;

        var transaction = new WalletTransaction(
            Id,
            WalletTransactionType.Withdrawal,
            amount,
            Balance,
            referenceId,
            description,
            externalReference);

        _transactions.Add(transaction);

        // Raise domain events
        AddDomainEvent(new WalletBalanceChangedEvent(
            Id,
            NationalNumber,
            WalletTransactionType.Withdrawal,
            previousBalance,
            Balance,
            amount,
            referenceId,
            DateTime.UtcNow));

        AddDomainEvent(new WalletTransactionCompletedEvent(
            transaction.Id,
            Id,
            NationalNumber,
            WalletTransactionType.Withdrawal,
            amount,
            Balance,
            referenceId,
            description,
            DateTime.UtcNow));

        return transaction;
    }

    /// <summary>
    /// انتقال وجه به کیف پول دیگر
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار مبلغ مشخصی را از این کیف پول به کیف پول مقصد انتقال می‌دهد
    /// و تراکنش انتقال را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر نیاز به انتقال وجه به کاربر دیگر یا حساب دیگری دارد،
    /// این رفتار برای انجام انتقال و ثبت تراکنش استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کیف پول باید فعال باشد.
    /// - موجودی کافی باید وجود داشته باشد.
    /// - مبلغ انتقال باید مثبت باشد.
    /// - کیف پول مقصد نباید همین کیف پول باشد.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: موجودی کافی وجود داشته باشد.
    /// - باید: کیف پول مقصد متفاوت باشد.
    /// - نباید: بیش از موجودی انتقال داده شود.
    /// - نباید: به خود انتقال داده شود.
    /// </remarks>
    public WalletTransaction TransferOut(
        Money amount,
        Guid destinationWalletId,
        string? referenceId = null,
        string? description = null,
        string? externalReference = null)
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Cannot transfer from inactive wallet");
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Transfer amount must be positive", nameof(amount));
        if (destinationWalletId == Id)
            throw new ArgumentException("Cannot transfer to the same wallet", nameof(destinationWalletId));
        if (Balance.IsLessThan(amount))
            throw new InvalidOperationException("Insufficient balance");

        var previousBalance = Balance;
        Balance = Balance.Subtract(amount);
        LastTransactionAt = DateTime.UtcNow;

        var transaction = new WalletTransaction(
            Id,
            WalletTransactionType.TransferOut,
            amount,
            Balance,
            referenceId,
            description,
            externalReference);

        transaction.AddMetadata("DestinationWalletId", destinationWalletId.ToString());

        _transactions.Add(transaction);

        // Raise domain events
        AddDomainEvent(new WalletBalanceChangedEvent(
            Id,
            NationalNumber,
            WalletTransactionType.TransferOut,
            previousBalance,
            Balance,
            amount,
            referenceId,
            DateTime.UtcNow));

        AddDomainEvent(new WalletTransactionCompletedEvent(
            transaction.Id,
            Id,
            NationalNumber,
            WalletTransactionType.TransferOut,
            amount,
            Balance,
            referenceId,
            description,
            DateTime.UtcNow));

        return transaction;
    }

    /// <summary>
    /// دریافت انتقال از کیف پول دیگر
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار مبلغ انتقال یافته از کیف پول مبدا را دریافت کرده و موجودی را افزایش می‌دهد
    /// و تراکنش دریافت انتقال را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر دیگری وجه به این کیف پول انتقال می‌دهد، این رفتار برای
    /// افزایش موجودی و ثبت تراکنش دریافت استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کیف پول باید فعال باشد.
    /// - مبلغ انتقال باید مثبت باشد.
    /// - موجودی جدید محاسبه و ثبت می‌شود.
    /// - تراکنش دریافت ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کیف پول فعال باشد.
    /// - باید: مبلغ مثبت باشد.
    /// - نباید: به کیف پول غیرفعال انتقال دریافت شود.
    /// - نباید: مبلغ صفر یا منفی دریافت شود.
    /// </remarks>
    public WalletTransaction TransferIn(
        Money amount,
        Guid sourceWalletId,
        string? referenceId = null,
        string? description = null,
        string? externalReference = null)
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Cannot receive transfer to inactive wallet");
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Transfer amount must be positive", nameof(amount));

        var previousBalance = Balance;
        Balance = Balance.Add(amount);
        LastTransactionAt = DateTime.UtcNow;

        var transaction = new WalletTransaction(
            Id,
            WalletTransactionType.TransferIn,
            amount,
            Balance,
            referenceId,
            description,
            externalReference);

        transaction.AddMetadata("SourceWalletId", sourceWalletId.ToString());

        _transactions.Add(transaction);

        // Raise domain events
        AddDomainEvent(new WalletBalanceChangedEvent(
            Id,
            NationalNumber,
            WalletTransactionType.TransferIn,
            previousBalance,
            Balance,
            amount,
            referenceId,
            DateTime.UtcNow));

        AddDomainEvent(new WalletTransactionCompletedEvent(
            transaction.Id,
            Id,
            NationalNumber,
            WalletTransactionType.TransferIn,
            amount,
            Balance,
            referenceId,
            description,
            DateTime.UtcNow));

        return transaction;
    }

    /// <summary>
    /// پرداخت فاکتور از کیف پول
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار مبلغ فاکتور را از کیف پول پرداخت کرده و موجودی را کاهش می‌دهد
    /// و تراکنش پرداخت را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر برای پرداخت فاکتور از موجودی کیف پول استفاده می‌کند،
    /// این رفتار برای کاهش موجودی و ثبت تراکنش پرداخت اجرا می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کیف پول باید فعال باشد.
    /// - موجودی کافی باید وجود داشته باشد.
    /// - مبلغ پرداخت باید مثبت باشد.
    /// - شناسه فاکتور اجباری است.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: موجودی کافی وجود داشته باشد.
    /// - باید: شناسه فاکتور ارائه شود.
    /// - نباید: بیش از موجودی پرداخت شود.
    /// - نباید: بدون شناسه فاکتور پرداخت شود.
    /// </remarks>
    public WalletTransaction PayBill(
        Money amount,
        Guid billId,
        string billNumber,
        string? referenceId = null,
        string? description = null,
        string? externalReference = null)
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Cannot pay from inactive wallet");
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Payment amount must be positive", nameof(amount));
        if (billId == Guid.Empty)
            throw new ArgumentException("Bill ID cannot be empty", nameof(billId));
        if (Balance.IsLessThan(amount))
            throw new InvalidOperationException("Insufficient balance");

        var previousBalance = Balance;
        Balance = Balance.Subtract(amount);
        LastTransactionAt = DateTime.UtcNow;

        var transaction = new WalletTransaction(
            Id,
            WalletTransactionType.Payment,
            amount,
            Balance,
            referenceId,
            description,
            externalReference);

        transaction.AddMetadata("BillId", billId.ToString());
        transaction.AddMetadata("BillNumber", billNumber);

        _transactions.Add(transaction);

        // Raise domain events
        AddDomainEvent(new WalletBalanceChangedEvent(
            Id,
            NationalNumber,
            WalletTransactionType.Payment,
            previousBalance,
            Balance,
            amount,
            referenceId,
            DateTime.UtcNow));

        AddDomainEvent(new WalletTransactionCompletedEvent(
            transaction.Id,
            Id,
            NationalNumber,
            WalletTransactionType.Payment,
            amount,
            Balance,
            referenceId,
            description,
            DateTime.UtcNow));

        return transaction;
    }

    /// <summary>
    /// دریافت بازگشت وجه به کیف پول
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار مبلغ بازگشت وجه را به کیف پول اضافه کرده و موجودی را افزایش می‌دهد
    /// و تراکنش بازگشت وجه را ثبت می‌نماید.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که فاکتور پرداخت شده برگشت می‌خورد یا تخفیف پسینی اعطا می‌شود،
    /// این رفتار برای افزایش موجودی و ثبت تراکنش بازگشت استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کیف پول باید فعال باشد.
    /// - مبلغ بازگشت باید مثبت باشد.
    /// - موجودی جدید محاسبه و ثبت می‌شود.
    /// - تراکنش بازگشت ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کیف پول فعال باشد.
    /// - باید: مبلغ مثبت باشد.
    /// - نباید: به کیف پول غیرفعال بازگشت داده شود.
    /// - نباید: مبلغ صفر یا منفی بازگشت داده شود.
    /// </remarks>
    public WalletTransaction ReceiveRefund(
        Money amount,
        Guid billId,
        string billNumber,
        string? referenceId = null,
        string? description = null,
        string? externalReference = null)
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Cannot receive refund to inactive wallet");
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Refund amount must be positive", nameof(amount));

        var previousBalance = Balance;
        Balance = Balance.Add(amount);
        LastTransactionAt = DateTime.UtcNow;

        var transaction = new WalletTransaction(
            Id,
            WalletTransactionType.Refund,
            amount,
            Balance,
            referenceId,
            description,
            externalReference);

        transaction.AddMetadata("BillId", billId.ToString());
        transaction.AddMetadata("BillNumber", billNumber);

        _transactions.Add(transaction);

        // Raise domain events
        AddDomainEvent(new WalletBalanceChangedEvent(
            Id,
            NationalNumber,
            WalletTransactionType.Refund,
            previousBalance,
            Balance,
            amount,
            referenceId,
            DateTime.UtcNow));

        AddDomainEvent(new WalletTransactionCompletedEvent(
            transaction.Id,
            Id,
            NationalNumber,
            WalletTransactionType.Refund,
            amount,
            Balance,
            referenceId,
            description,
            DateTime.UtcNow));

        return transaction;
    }

    /// <summary>
    /// تنظیم مجدد موجودی (تنظیمات اداری)
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار موجودی کیف پول را به مبلغ مشخص تنظیم می‌کند و تراکنش تنظیم مجدد
    /// را ثبت می‌نماید. این عمل معمولاً برای اصلاح خطاها یا تنظیمات اداری استفاده می‌شود.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به اصلاح موجودی به دلیل خطای سیستم، تنظیمات اداری یا
    /// سایر موارد خاص وجود دارد، این رفتار استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کیف پول باید فعال باشد.
    /// - مبلغ جدید نباید منفی باشد.
    /// - تفاوت موجودی محاسبه و ثبت می‌شود.
    /// - تراکنش تنظیم مجدد ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: مبلغ جدید معتبر باشد.
    /// - باید: دلیل تنظیم مجدد ثبت شود.
    /// - نباید: مبلغ منفی تنظیم شود.
    /// - نباید: بدون دلیل تنظیم مجدد انجام شود.
    /// </remarks>
    public WalletTransaction AdjustBalance(
        Money newBalance,
        string reason,
        string? referenceId = null,
        string? description = null,
        string? externalReference = null)
    {
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Cannot adjust inactive wallet");
        if (newBalance.AmountRials < 0)
            throw new ArgumentException("New balance cannot be negative", nameof(newBalance));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Adjustment reason is required", nameof(reason));

        var previousBalance = Balance;
        var difference = Money.FromRials(newBalance.AmountRials - Balance.AmountRials);
        Balance = newBalance;
        LastTransactionAt = DateTime.UtcNow;

        var transaction = new WalletTransaction(
            Id,
            WalletTransactionType.Adjustment,
            difference,
            Balance,
            referenceId,
            description,
            externalReference);

        transaction.AddMetadata("Reason", reason);
        transaction.AddMetadata("PreviousBalance", previousBalance.AmountRials.ToString());

        _transactions.Add(transaction);

        // Raise domain events
        AddDomainEvent(new WalletBalanceChangedEvent(
            Id,
            NationalNumber,
            WalletTransactionType.Adjustment,
            previousBalance,
            Balance,
            difference,
            referenceId,
            DateTime.UtcNow));

        AddDomainEvent(new WalletTransactionCompletedEvent(
            transaction.Id,
            Id,
            NationalNumber,
            WalletTransactionType.Adjustment,
            difference,
            Balance,
            referenceId,
            description,
            DateTime.UtcNow));

        return transaction;
    }

    /// <summary>
    /// تغییر وضعیت کیف پول
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار وضعیت کیف پول را تغییر می‌دهد و دلیل تغییر را ثبت می‌نماید.
    /// این عمل برای مدیریت دسترسی و کنترل کیف پول استفاده می‌شود.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به تعلیق، مسدود کردن یا بستن کیف پول وجود دارد،
    /// این رفتار برای تغییر وضعیت و ثبت دلیل استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - وضعیت جدید باید معتبر باشد.
    /// - دلیل تغییر در صورت وجود ثبت می‌شود.
    /// - رویداد تغییر وضعیت ایجاد می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: وضعیت جدید معتبر باشد.
    /// - باید: دلیل تغییر ثبت شود.
    /// - نباید: وضعیت نامعتبر تنظیم شود.
    /// - نباید: بدون دلیل وضعیت تغییر کند.
    /// </remarks>
    public void ChangeStatus(WalletStatus newStatus, string? reason = null)
    {
        if (Status == newStatus)
            return; // No change needed

        var previousStatus = Status;
        Status = newStatus;

        if (!string.IsNullOrWhiteSpace(reason))
        {
            Metadata["StatusChangeReason"] = reason;
        }
        Metadata["StatusChangedAt"] = DateTime.UtcNow.ToString("O");

        // Raise domain event
        AddDomainEvent(new WalletStatusChangedEvent(
            Id,
            NationalNumber,
            previousStatus,
            Status,
            reason,
            DateTime.UtcNow));
    }

    /// <summary>
    /// تعلیق موقت کیف پول
    /// </summary>
    public void Suspend(string? reason = null)
    {
        ChangeStatus(WalletStatus.Suspended, reason ?? "Wallet suspended");
    }

    /// <summary>
    /// مسدود کردن کیف پول (نیاز به مداخله دستی)
    /// </summary>
    public void Freeze(string? reason = null)
    {
        ChangeStatus(WalletStatus.Frozen, reason ?? "Wallet frozen");
    }

    /// <summary>
    /// فعال کردن کیف پول
    /// </summary>
    public void Activate(string? reason = null)
    {
        ChangeStatus(WalletStatus.Active, reason ?? "Wallet activated");
    }

    /// <summary>
    /// بستن دائمی کیف پول
    /// </summary>
    public void Close(string? reason = null)
    {
        if (Balance.AmountRials > 0)
            throw new InvalidOperationException("Cannot close wallet with remaining balance");

        ChangeStatus(WalletStatus.Closed, reason ?? "Wallet closed");
    }

    /// <summary>
    /// بررسی اینکه آیا کیف پول فعال است
    /// </summary>
    public bool IsActive()
    {
        return Status == WalletStatus.Active;
    }

    /// <summary>
    /// بررسی اینکه آیا موجودی کافی برای مبلغ مشخص وجود دارد
    /// </summary>
    public bool HasSufficientBalance(Money amount)
    {
        return Balance.IsGreaterThan(amount) || Balance.Equals(amount);
    }

    /// <summary>
    /// دریافت موجودی قابل استفاده (موجودی منهای مبالغ مسدود شده)
    /// </summary>
    public Money GetAvailableBalance()
    {
        // For now, available balance equals current balance
        // In future, this can be extended to account for frozen amounts
        return Balance;
    }

    /// <summary>
    /// دریافت تعداد تراکنش‌ها
    /// </summary>
    public int GetTransactionCount()
    {
        return _transactions.Count;
    }

    /// <summary>
    /// دریافت تراکنش‌های اخیر
    /// </summary>
    public IEnumerable<WalletTransaction> GetRecentTransactions(int count = 10)
    {
        return _transactions
            .OrderByDescending(t => t.CreatedAt)
            .Take(count);
    }
}
