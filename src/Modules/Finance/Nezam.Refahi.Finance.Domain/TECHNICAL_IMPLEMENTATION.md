# 🔧 Finance Domain Technical Implementation Guide

## 📋 Overview

This document provides detailed technical implementation guidelines for the Finance Domain Context, including architecture patterns, coding standards, and best practices.

## 🏗️ Architecture Patterns

### Domain-Driven Design (DDD)
The Finance domain follows DDD principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    FINANCE DOMAIN                           │
├─────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │   Entities  │  │ Value Objects│  │   Events    │        │
│  │             │  │             │  │             │        │
│  │ • Bill      │  │ • Money     │  │ • BillEvents│        │
│  │ • Wallet    │  │             │  │ • WalletEvts│        │
│  │ • Payment   │  │             │  │ • PaymentEvts│       │
│  │ • Refund    │  │             │  │ • RefundEvts│        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
│                                                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │  Aggregates │  │   Services  │  │ Repositories│        │
│  │             │  │             │  │             │        │
│  │ • Bill      │  │ • Wallet    │  │ • IBillRepo │        │
│  │ • Wallet    │  │   Domain    │  │ • IWalletRepo│       │
│  │ • Payment   │  │   Service   │  │ • IPaymentRepo│      │
│  │ • Refund    │  │             │  │ • IRefundRepo│      │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└─────────────────────────────────────────────────────────────┘
```

### Aggregate Design
Each aggregate represents a consistency boundary:

#### Bill Aggregate
```csharp
public sealed class Bill : FullAggregateRoot<Guid>
{
    // Root entity properties
    public string BillNumber { get; private set; }
    public BillStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    
    // Collection of child entities
    private readonly List<BillItem> _items = new();
    public IReadOnlyCollection<BillItem> Items => _items.AsReadOnly();
    
    // Business methods
    public void AddItem(BillItem item) { /* Implementation */ }
    public void Issue() { /* Implementation */ }
    public void MarkAsPaid(Money amount) { /* Implementation */ }
    public void Cancel() { /* Implementation */ }
}
```

#### Wallet Aggregate
```csharp
public sealed class Wallet : FullAggregateRoot<Guid>
{
    // Root entity properties
    public string NationalNumber { get; private set; }
    public Money Balance { get; private set; }
    public WalletStatus Status { get; private set; }
    
    // Collection of child entities
    private readonly List<WalletTransaction> _transactions = new();
    public IReadOnlyCollection<WalletTransaction> Transactions => _transactions.AsReadOnly();
    
    // Business methods
    public WalletTransaction Deposit(Money amount, string? referenceId = null) { /* Implementation */ }
    public WalletTransaction Withdraw(Money amount, string? referenceId = null) { /* Implementation */ }
    public void Suspend() { /* Implementation */ }
    public void Activate() { /* Implementation */ }
}
```

## 🎯 Value Objects

### Money Value Object
```csharp
public record Money
{
    public long AmountRials { get; init; }
    public string Currency { get; init; } = "IRR";
    
    public static Money FromRials(long amountRials) => new() { AmountRials = amountRials };
    public static Money Zero => new() { AmountRials = 0 };
    
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add different currencies");
        return new Money { AmountRials = left.AmountRials + right.AmountRials, Currency = left.Currency };
    }
    
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract different currencies");
        return new Money { AmountRials = left.AmountRials - right.AmountRials, Currency = left.Currency };
    }
}
```

## 🎭 Domain Events

### Event Structure
```csharp
public abstract class DomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType { get; } = GetType().Name;
}

public class BillFullyPaidEvent : DomainEvent
{
    public Guid BillId { get; }
    public string BillNumber { get; }
    public string ReferenceId { get; }
    public string ReferenceType { get; }
    public string UserNationalNumber { get; }
    public Money TotalAmount { get; }
    public Money PaidAmount { get; }
    public DateTime FullyPaidDate { get; }
    public int PaymentCount { get; }
    public string? LastPaymentMethod { get; }
    public string? LastGateway { get; }
    
    public BillFullyPaidEvent(
        Guid billId,
        string billNumber,
        string referenceId,
        string referenceType,
        string userNationalNumber,
        Money totalAmount,
        Money paidAmount,
        DateTime fullyPaidDate,
        int paymentCount,
        string? lastPaymentMethod = null,
        string? lastGateway = null)
    {
        BillId = billId;
        BillNumber = billNumber;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
        UserNationalNumber = userNationalNumber;
        TotalAmount = totalAmount;
        PaidAmount = paidAmount;
        FullyPaidDate = fullyPaidDate;
        PaymentCount = paymentCount;
        LastPaymentMethod = lastPaymentMethod;
        LastGateway = lastGateway;
    }
}
```

### Event Raising Pattern
```csharp
public class Bill : FullAggregateRoot<Guid>
{
    public void MarkAsPaid(Money amount)
    {
        if (Status != BillStatus.Issued)
            throw new InvalidOperationException("Can only mark issued bills as paid");
        
        PaidAmount = PaidAmount + amount;
        RemainingAmount = TotalAmount - PaidAmount;
        
        if (RemainingAmount.AmountRials <= 0)
        {
            Status = BillStatus.Paid;
            FullyPaidDate = DateTime.UtcNow;
            
            // Raise domain event
            AddDomainEvent(new BillFullyPaidEvent(
                Id,
                BillNumber,
                ReferenceId,
                BillType,
                UserNationalNumber,
                TotalAmount,
                PaidAmount,
                FullyPaidDate.Value,
                PaymentCount,
                LastPaymentMethod,
                LastGateway));
        }
    }
}
```

## 🔧 Domain Services

### Stateless Domain Service
```csharp
public sealed class WalletDomainService
{
    public WalletDomainService()
    {
        // No dependencies - stateless service
    }
    
    public WalletBalanceAnalysis AnalyzeBalanceHistory(
        Wallet wallet, 
        DateTime? fromDate = null, 
        DateTime? toDate = null)
    {
        var startDate = fromDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = toDate ?? DateTime.UtcNow;
        
        var transactions = wallet.Transactions
            .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
            .OrderBy(t => t.CreatedAt)
            .ToList();
        
        var totalDeposits = transactions
            .Where(t => t.IsDeposit())
            .Sum(t => t.Amount.AmountRials);
        
        var totalWithdrawals = transactions
            .Where(t => t.IsWithdrawal())
            .Sum(t => t.Amount.AmountRials);
        
        var transactionCount = transactions.Count;
        var averageTransactionAmount = transactionCount > 0 
            ? transactions.Average(t => t.Amount.AmountRials) 
            : 0;
        
        return new WalletBalanceAnalysis
        {
            TotalDeposits = totalDeposits,
            TotalWithdrawals = totalWithdrawals,
            NetChange = totalDeposits - totalWithdrawals,
            TransactionCount = transactionCount,
            AverageTransactionAmount = averageTransactionAmount,
            AnalysisPeriod = new DateRange(startDate, endDate)
        };
    }
    
    public TransactionLimitValidation ValidateTransactionLimits(
        Wallet wallet, 
        Money amount, 
        WalletTransactionType transactionType)
    {
        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        
        var usedToday = wallet.Transactions
            .Where(t => t.CreatedAt.Date == today)
            .Where(t => t.IsWithdrawal())
            .Sum(t => t.Amount.AmountRials);
        
        var usedThisMonth = wallet.Transactions
            .Where(t => t.CreatedAt >= monthStart)
            .Where(t => t.IsWithdrawal())
            .Sum(t => t.Amount.AmountRials);
        
        var dailyLimit = GetDailyLimit(wallet);
        var monthlyLimit = GetMonthlyLimit(wallet);
        
        var isValid = usedToday + amount.AmountRials <= dailyLimit &&
                     usedThisMonth + amount.AmountRials <= monthlyLimit;
        
        var warnings = new List<string>();
        if (usedToday + amount.AmountRials > dailyLimit * 0.8)
            warnings.Add("Approaching daily limit");
        if (usedThisMonth + amount.AmountRials > monthlyLimit * 0.8)
            warnings.Add("Approaching monthly limit");
        
        return new TransactionLimitValidation
        {
            IsValid = isValid,
            DailyLimit = dailyLimit,
            MonthlyLimit = monthlyLimit,
            UsedToday = usedToday,
            UsedThisMonth = usedThisMonth,
            RemainingDaily = dailyLimit - usedToday,
            RemainingMonthly = monthlyLimit - usedThisMonth,
            Warnings = warnings
        };
    }
    
    private static long GetDailyLimit(Wallet wallet)
    {
        // Business logic for daily limits
        return 10000000; // 10M rials
    }
    
    private static long GetMonthlyLimit(Wallet wallet)
    {
        // Business logic for monthly limits
        return 100000000; // 100M rials
    }
}
```

## 🗄️ Repository Pattern

### Repository Interface
```csharp
public interface IWalletRepository : IRepository<Wallet, Guid>
{
    Task<Wallet?> GetByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Wallet>> GetByStatusAsync(WalletStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Wallet>> GetActiveWalletsAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default);
}
```

### Repository Implementation
```csharp
public class WalletRepository : EfRepository<FinanceDbContext, Wallet, Guid>, IWalletRepository
{
    public WalletRepository(FinanceDbContext context) : base(context)
    {
    }
    
    public async Task<Wallet?> GetByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Wallets
            .FirstOrDefaultAsync(w => w.NationalNumber == nationalNumber, cancellationToken);
    }
    
    public async Task<IEnumerable<Wallet>> GetByStatusAsync(WalletStatus status, CancellationToken cancellationToken = default)
    {
        return await Context.Wallets
            .Where(w => w.Status == status)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<IEnumerable<Wallet>> GetActiveWalletsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Wallets
            .Where(w => w.Status == WalletStatus.Active)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<bool> ExistsByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Wallets
            .AnyAsync(w => w.NationalNumber == nationalNumber, cancellationToken);
    }
    
    protected override IQueryable<Wallet> PrepareQuery(IQueryable<Wallet> query)
    {
        return query
            .Include(w => w.Transactions);
    }
}
```

## 🏷️ Enum Implementation

### Enum Structure
```csharp
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WalletStatus
{
    [Description("فعال")]
    Active = 1,
    
    [Description("معلق")]
    Suspended = 2,
    
    [Description("بسته شده")]
    Closed = 3
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WalletTransactionType
{
    [Description("واریز")]
    Deposit = 1,
    
    [Description("برداشت")]
    Withdrawal = 2,
    
    [Description("انتقال ورودی")]
    TransferIn = 3,
    
    [Description("انتقال خروجی")]
    TransferOut = 4,
    
    [Description("پرداخت")]
    Payment = 5,
    
    [Description("بازگشت وجه")]
    Refund = 6,
    
    [Description("تنظیم دستی")]
    Adjustment = 7
}
```

## 🔄 Entity Framework Configuration

### Entity Configuration
```csharp
public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        // Table configuration
        builder.ToTable("Wallets", "Finance");
        builder.HasKey(x => x.Id);
        
        // Properties configuration
        builder.Property(x => x.NationalNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(x => x.Balance)
            .IsRequired()
            .HasConversion(
                v => new { v.AmountRials, v.Currency },
                v => new Money(v.AmountRials, v.Currency));
        
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();
        
        builder.Property(x => x.WalletName)
            .HasMaxLength(100);
        
        builder.Property(x => x.Description)
            .HasMaxLength(500);
        
        builder.Property(x => x.LastTransactionAt);
        
        builder.Property(x => x.Metadata)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>());
        
        // Indexes
        builder.HasIndex(x => x.NationalNumber)
            .IsUnique()
            .HasDatabaseName("IX_Wallets_NationalNumber");
        
        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Wallets_Status");
        
        builder.HasIndex(x => x.LastTransactionAt)
            .HasDatabaseName("IX_Wallets_LastTransactionAt");
        
        // Relationships
        builder.HasMany(x => x.Transactions)
            .WithOne(x => x.Wallet)
            .HasForeignKey(x => x.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Audit fields
        builder.ConfigureAuditableEntity();
    }
}
```

## 🎯 Business Rule Implementation

### Validation Patterns
```csharp
public class Wallet : FullAggregateRoot<Guid>
{
    public WalletTransaction Deposit(Money amount, string? referenceId = null, string? description = null, string? externalReference = null, Dictionary<string, string>? metadata = null)
    {
        // Business rule validation
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Cannot deposit to inactive wallet");
        
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Deposit amount must be positive", nameof(amount));
        
        // Create transaction
        var newBalance = Balance + amount;
        var transaction = new WalletTransaction(
            walletId: Id,
            transactionType: WalletTransactionType.Deposit,
            amount: amount,
            balanceAfter: newBalance,
            referenceId: referenceId,
            description: description,
            externalReference: externalReference,
            metadata: metadata);
        
        // Update aggregate state
        _transactions.Add(transaction);
        Balance = newBalance;
        LastTransactionAt = DateTime.UtcNow;
        
        // Raise domain event
        AddDomainEvent(new WalletBalanceChangedEvent(
            Id,
            NationalNumber,
            Balance,
            LastTransactionAt.Value,
            transaction.Id));
        
        return transaction;
    }
    
    public WalletTransaction Withdraw(Money amount, string? referenceId = null, string? description = null, string? externalReference = null, Dictionary<string, string>? metadata = null)
    {
        // Business rule validation
        if (Status != WalletStatus.Active)
            throw new InvalidOperationException("Cannot withdraw from inactive wallet");
        
        if (amount.AmountRials <= 0)
            throw new ArgumentException("Withdrawal amount must be positive", nameof(amount));
        
        if (Balance.AmountRials < amount.AmountRials)
            throw new InvalidOperationException("Insufficient balance");
        
        // Create transaction
        var newBalance = Balance - amount;
        var transaction = new WalletTransaction(
            walletId: Id,
            transactionType: WalletTransactionType.Withdrawal,
            amount: amount,
            balanceAfter: newBalance,
            referenceId: referenceId,
            description: description,
            externalReference: externalReference,
            metadata: metadata);
        
        // Update aggregate state
        _transactions.Add(transaction);
        Balance = newBalance;
        LastTransactionAt = DateTime.UtcNow;
        
        // Raise domain event
        AddDomainEvent(new WalletBalanceChangedEvent(
            Id,
            NationalNumber,
            Balance,
            LastTransactionAt.Value,
            transaction.Id));
        
        return transaction;
    }
}
```

## 🔐 Security Implementation

### Data Protection
```csharp
public class Wallet : FullAggregateRoot<Guid>
{
    private string _nationalNumber = null!;
    
    public string NationalNumber
    {
        get => _nationalNumber;
        private set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("National number cannot be empty", nameof(value));
            
            // Validate national number format
            if (!IsValidNationalNumber(value))
                throw new ArgumentException("Invalid national number format", nameof(value));
            
            _nationalNumber = value.Trim();
        }
    }
    
    private static bool IsValidNationalNumber(string nationalNumber)
    {
        // Iranian national number validation
        if (nationalNumber.Length != 10)
            return false;
        
        if (!nationalNumber.All(char.IsDigit))
            return false;
        
        // Additional validation logic
        return true;
    }
}
```

### Access Control
```csharp
public interface IWalletRepository : IRepository<Wallet, Guid>
{
    Task<Wallet?> GetByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default);
    
    // Security: Only allow access to user's own wallet
    Task<Wallet?> GetUserWalletAsync(string userNationalNumber, CancellationToken cancellationToken = default);
}
```

## 📊 Performance Optimization

### Query Optimization
```csharp
public class WalletRepository : EfRepository<FinanceDbContext, Wallet, Guid>, IWalletRepository
{
    public async Task<Wallet?> GetByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default)
    {
        return await Context.Wallets
            .AsNoTracking() // Read-only query
            .FirstOrDefaultAsync(w => w.NationalNumber == nationalNumber, cancellationToken);
    }
    
    public async Task<Wallet?> GetWithTransactionsAsync(Guid walletId, CancellationToken cancellationToken = default)
    {
        return await Context.Wallets
            .Include(w => w.Transactions.OrderByDescending(t => t.CreatedAt).Take(50)) // Limit transactions
            .FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);
    }
    
    public async Task<IEnumerable<Wallet>> GetActiveWalletsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Wallets
            .Where(w => w.Status == WalletStatus.Active)
            .OrderBy(w => w.LastTransactionAt)
            .Take(1000) // Limit results
            .ToListAsync(cancellationToken);
    }
}
```

### Caching Strategy
```csharp
public class CachedWalletRepository : IWalletRepository
{
    private readonly IWalletRepository _repository;
    private readonly IMemoryCache _cache;
    
    public CachedWalletRepository(IWalletRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }
    
    public async Task<Wallet?> GetByNationalNumberAsync(string nationalNumber, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"wallet:{nationalNumber}";
        
        if (_cache.TryGetValue(cacheKey, out Wallet? cachedWallet))
            return cachedWallet;
        
        var wallet = await _repository.GetByNationalNumberAsync(nationalNumber, cancellationToken);
        
        if (wallet != null)
        {
            _cache.Set(cacheKey, wallet, TimeSpan.FromMinutes(5));
        }
        
        return wallet;
    }
}
```

## 🧪 Testing Patterns

### Unit Testing
```csharp
[Test]
public void Deposit_ShouldIncreaseBalance_WhenValidAmount()
{
    // Arrange
    var wallet = new Wallet("1234567890", Money.Zero);
    var depositAmount = Money.FromRials(100000);
    
    // Act
    var transaction = wallet.Deposit(depositAmount, "REF001", "Test deposit");
    
    // Assert
    Assert.That(wallet.Balance.AmountRials, Is.EqualTo(100000));
    Assert.That(transaction.TransactionType, Is.EqualTo(WalletTransactionType.Deposit));
    Assert.That(transaction.Amount.AmountRials, Is.EqualTo(100000));
    Assert.That(wallet.Transactions.Count, Is.EqualTo(1));
}

[Test]
public void Withdraw_ShouldThrowException_WhenInsufficientBalance()
{
    // Arrange
    var wallet = new Wallet("1234567890", Money.FromRials(50000));
    var withdrawalAmount = Money.FromRials(100000);
    
    // Act & Assert
    Assert.Throws<InvalidOperationException>(() => 
        wallet.Withdraw(withdrawalAmount, "REF002", "Test withdrawal"));
}
```

### Integration Testing
```csharp
[Test]
public async Task WalletChargingFlow_ShouldCompleteSuccessfully()
{
    // Arrange
    var userNationalNumber = "1234567890";
    var wallet = new Wallet(userNationalNumber, Money.Zero);
    var depositAmount = Money.FromRials(100000);
    
    // Act
    var walletDeposit = new WalletDeposit(wallet.Id, userNationalNumber, depositAmount);
    var bill = new Bill("Test Bill", walletDeposit.Id.ToString(), "WalletDeposit", userNationalNumber);
    
    walletDeposit.SetBillId(bill.Id);
    bill.Issue();
    bill.MarkAsPaid(depositAmount);
    walletDeposit.Complete();
    
    var transaction = wallet.Deposit(depositAmount, walletDeposit.Id.ToString());
    
    // Assert
    Assert.That(wallet.Balance.AmountRials, Is.EqualTo(100000));
    Assert.That(walletDeposit.Status, Is.EqualTo(WalletDepositStatus.Completed));
    Assert.That(bill.Status, Is.EqualTo(BillStatus.Paid));
    Assert.That(transaction.TransactionType, Is.EqualTo(WalletTransactionType.Deposit));
}
```

## 📚 Best Practices

### 1. Aggregate Design
- Keep aggregates small and focused
- Maintain consistency boundaries
- Use domain events for cross-aggregate communication
- Avoid loading entire aggregates when only parts are needed

### 2. Domain Events
- Raise events for all significant state changes
- Keep events immutable
- Include all necessary data in events
- Use events for side effects and notifications

### 3. Repository Pattern
- Define clear interfaces
- Use specific methods for common queries
- Implement proper error handling
- Use async/await consistently

### 4. Business Rules
- Enforce rules at the domain level
- Use validation in constructors and methods
- Provide clear error messages
- Maintain data integrity

### 5. Performance
- Use appropriate indexing
- Implement caching where beneficial
- Optimize queries for common scenarios
- Monitor and measure performance

---

This technical implementation guide provides comprehensive guidelines for implementing the Finance Domain Context following best practices and architectural patterns.
