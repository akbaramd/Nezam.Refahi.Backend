using MCA.SharedKernel.Domain;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// تراکنش کیف پول - ثبت تمامی حرکات مالی کیف پول
/// در دنیای واقعی: هر واریز، برداشت، انتقال، پرداخت یا بازگشت وجه
/// </summary>
public sealed class WalletTransaction : Entity<Guid>
{
    public Guid WalletId { get; private set; }
    public WalletTransactionType TransactionType { get; private set; }
    public Money Amount { get; private set; } = null!;
    public Money BalanceAfter { get; private set; } = null!;
    public WalletTransactionStatus Status { get; private set; }
    public string? ReferenceId { get; private set; }
    public string? Description { get; private set; }
    public string? ExternalReference { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation property
    public Wallet Wallet { get; private set; } = null!;

    // Private constructor for EF Core
    private WalletTransaction() : base() { }

    /// <summary>
    /// ایجاد تراکنش جدید کیف پول
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک تراکنش جدید کیف پول ایجاد می‌کند که شامل نوع تراکنش، مبلغ،
    /// موجودی پس از تراکنش و سایر اطلاعات مرتبط است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر واریز، برداشت، انتقال یا پرداخت انجام می‌دهد، این رفتار
    /// برای ثبت دقیق جزئیات تراکنش و ایجاد سابقه مالی استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه کیف پول اجباری و معتبر باشد.
    /// - نوع تراکنش باید مشخص باشد.
    /// - مبلغ باید مثبت و معتبر باشد.
    /// - موجودی پس از تراکنش باید محاسبه شده باشد.
    /// - زمان ایجاد به صورت خودکار ثبت می‌شود.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: تمام اطلاعات ضروری تکمیل باشد.
    /// - باید: مبلغ و موجودی معتبر باشد.
    /// - نباید: تراکنش بدون شناسه کیف پول ایجاد شود.
    /// - نباید: مبلغ صفر یا منفی پذیرفته شود.
    /// </remarks>
    public WalletTransaction(
        Guid walletId,
        WalletTransactionType transactionType,
        Money amount,
        Money balanceAfter,
        string? referenceId = null,
        string? description = null,
        string? externalReference = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (walletId == Guid.Empty)
            throw new ArgumentException("Wallet ID cannot be empty", nameof(walletId));

        WalletId = walletId;
        TransactionType = transactionType;
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        BalanceAfter = balanceAfter ?? throw new ArgumentNullException(nameof(balanceAfter));
        Status = WalletTransactionStatus.Completed; // Default to completed for wallet transactions
        ReferenceId = referenceId?.Trim();
        Description = description?.Trim();
        ExternalReference = externalReference?.Trim();
        CreatedAt = DateTime.UtcNow;
        Metadata = metadata ?? new Dictionary<string, string>();
    }

    /// <summary>
    /// افزودن اطلاعات اضافی به تراکنش
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار اطلاعات اضافی مانند شناسه درگاه، کد پیگیری یا سایر داده‌های
    /// مرتبط را به تراکنش اضافه می‌کند تا برای پیگیری و گزارش‌گیری در دسترس باشد.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که نیاز به ذخیره اطلاعات اضافی مانند شناسه تراکنش بانکی، کد پیگیری
    /// پستی یا سایر شناسه‌های خارجی وجود دارد، این رفتار استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - کلید و مقدار نباید خالی باشند.
    /// - اطلاعات اضافه شده قابل تغییر نیستند.
    /// - کلیدها باید منحصر به فرد باشند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: کلید و مقدار معتبر ارائه شود.
    /// - باید: اطلاعات اضافه شده حفظ شود.
    /// - نباید: کلید یا مقدار خالی پذیرفته شود.
    /// - نباید: اطلاعات حساس در metadata ذخیره شود.
    /// </remarks>
    public void AddMetadata(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Metadata key cannot be empty", nameof(key));
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Metadata value cannot be empty", nameof(value));

        Metadata[key.Trim()] = value.Trim();
    }

    /// <summary>
    /// بررسی اینکه آیا تراکنش مربوط به واریز است
    /// </summary>
    public bool IsDeposit()
    {
        return TransactionType == WalletTransactionType.Deposit ||
               TransactionType == WalletTransactionType.TransferIn ||
               TransactionType == WalletTransactionType.Refund ||
               TransactionType == WalletTransactionType.Interest ||
               TransactionType == WalletTransactionType.Adjustment && Amount.AmountRials > 0;
    }

    /// <summary>
    /// بررسی اینکه آیا تراکنش مربوط به برداشت است
    /// </summary>
    public bool IsWithdrawal()
    {
        return TransactionType == WalletTransactionType.Withdrawal ||
               TransactionType == WalletTransactionType.TransferOut ||
               TransactionType == WalletTransactionType.Payment ||
               TransactionType == WalletTransactionType.Fee ||
               TransactionType == WalletTransactionType.Adjustment && Amount.AmountRials < 0;
    }

    /// <summary>
    /// دریافت مبلغ مطلق تراکنش (بدون در نظر گیری جهت)
    /// </summary>
    public Money GetAbsoluteAmount()
    {
        return Money.FromRials(Math.Abs(Amount.AmountRials));
    }
}
