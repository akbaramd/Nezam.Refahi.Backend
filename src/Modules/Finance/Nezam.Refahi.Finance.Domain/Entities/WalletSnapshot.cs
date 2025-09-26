using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// کیف پول اسنپ‌شات - تصویر روزانه از موجودی کیف پول
/// در دنیای واقعی: برای بهینه‌سازی عملکرد و ایجاد تاریخچه کامل موجودی
/// </summary>
public sealed class WalletSnapshot : FullAggregateRoot<Guid>
{
    public Guid WalletId { get; private set; }
    public Guid ExternalUserId { get; private set; }
    public Money Balance { get; private set; } = null!;
    public DateTime SnapshotDate { get; private set; }
    public int TransactionCount { get; private set; }
    public Money TotalDeposits { get; private set; } = null!;
    public Money TotalWithdrawals { get; private set; } = null!;
    public Money NetChange { get; private set; } = null!;
    public DateTime? LastTransactionAt { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; } = new();

    // Navigation properties
    public Wallet Wallet { get; private set; } = null!;

    // Private constructor for EF Core
    private WalletSnapshot() : base() { }

    /// <summary>
    /// ایجاد اسنپ‌شات جدید از کیف پول
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این رفتار یک اسنپ‌شات از وضعیت فعلی کیف پول ایجاد می‌کند که شامل
    /// موجودی، تعداد تراکنش‌ها، مجموع واریزها و برداشت‌ها است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// برای بهینه‌سازی عملکرد، به جای محاسبه موجودی از تمام تراکنش‌ها،
    /// از آخرین اسنپ‌شات و تراکنش‌های بعد از آن استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - شناسه کیف پول اجباری و معتبر باشد.
    /// - تاریخ اسنپ‌شات باید معتبر باشد.
    /// - موجودی نمی‌تواند منفی باشد.
    /// - تمام مقادیر مالی باید معتبر باشند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: شناسه کیف پول و تاریخ معتبر ارائه شود.
    /// - باید: موجودی مثبت یا صفر باشد.
    /// - نباید: اسنپ‌شات بدون کیف پول ایجاد شود.
    /// - نباید: مقادیر مالی منفی باشند.
    /// </remarks>
    public WalletSnapshot(
        Guid walletId,
        Guid externalUserId,
        Money balance,
        DateTime snapshotDate,
        int transactionCount,
        Money totalDeposits,
        Money totalWithdrawals,
        DateTime? lastTransactionAt = null,
        Dictionary<string, string>? metadata = null)
        : base(Guid.NewGuid())
    {
        if (walletId == Guid.Empty)
            throw new ArgumentException("Wallet ID cannot be empty", nameof(walletId));
        if (externalUserId == Guid.Empty)
            throw new ArgumentException("External user ID cannot be empty", nameof(externalUserId));
        if (balance.AmountRials < 0)
            throw new ArgumentException("Balance cannot be negative", nameof(balance));
        if (totalDeposits.AmountRials < 0)
            throw new ArgumentException("Total deposits cannot be negative", nameof(totalDeposits));
        if (totalWithdrawals.AmountRials < 0)
            throw new ArgumentException("Total withdrawals cannot be negative", nameof(totalWithdrawals));

        WalletId = walletId;
        ExternalUserId = externalUserId;
        Balance = balance;
        SnapshotDate = snapshotDate.Date; // Only date part, no time
        TransactionCount = transactionCount;
        TotalDeposits = totalDeposits;
        TotalWithdrawals = totalWithdrawals;
        NetChange = totalDeposits.Subtract(totalWithdrawals);
        LastTransactionAt = lastTransactionAt;
        Metadata = metadata ?? new Dictionary<string, string>();

        // Raise domain event
        AddDomainEvent(new WalletSnapshotCreatedEvent(
            Id,
            WalletId,
            ExternalUserId,
            Balance,
            SnapshotDate,
            TransactionCount));
    }

    /// <summary>
    /// افزودن اطلاعات اضافی به اسنپ‌شات
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
    /// بررسی آیا اسنپ‌شات برای تاریخ مشخص معتبر است
    /// </summary>
    public bool IsValidForDate(DateTime date)
    {
        return SnapshotDate.Date <= date.Date;
    }

    /// <summary>
    /// محاسبه موجودی بر اساس اسنپ‌شات و تراکنش‌های بعد از آن
    /// </summary>
    public Money CalculateBalanceWithTransactions(IEnumerable<WalletTransaction> transactionsAfterSnapshot)
    {
        var balanceAfterTransactions = Balance;
        
        foreach (var transaction in transactionsAfterSnapshot)
        {
            if (transaction.IsIn())
            {
                balanceAfterTransactions = balanceAfterTransactions.Add(transaction.Amount);
            }
            else if (transaction.IsOut())
            {
                balanceAfterTransactions = balanceAfterTransactions.Subtract(transaction.Amount);
            }
        }

        return balanceAfterTransactions;
    }
}
