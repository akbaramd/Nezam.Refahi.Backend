using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Services;

/// <summary>
/// Domain service for all wallet-related business logic
/// This service is stateless and handles complex business operations that don't belong to a single aggregate
/// </summary>
public sealed class WalletDomainService
{
    #region Transfer Operations

    /// <summary>
    /// Transfer money between two wallets atomically
    /// </summary>
    /// <remarks>
    /// <para>توضیح رفتار:</para>
    /// این سرویس انتقال وجه بین دو کیف پول را به صورت اتمیک انجام می‌دهد
    /// و شامل اعتبارسنجی، محاسبه کارمزد و ایجاد تراکنش‌های مربوطه است.
    ///
    /// <para>کاربرد در دنیای واقعی:</para>
    /// زمانی که کاربر نیاز به انتقال وجه به کاربر دیگر دارد، این سرویس
    /// برای هماهنگی بین کیف پول مبدا و مقصد استفاده می‌شود.
    ///
    /// <para>قوانین:</para>
    /// - هر دو کیف پول باید فعال باشند.
    /// - موجودی کافی در کیف پول مبدا وجود داشته باشد.
    /// - کیف پول مبدا و مقصد متفاوت باشند.
    /// - کارمزد انتقال در صورت وجود محاسبه شود.
    /// - تراکنش‌ها به صورت اتمیک ایجاد شوند.
    ///
    /// <para>بایدها و نبایدها:</para>
    /// - باید: اعتبارسنجی کامل انجام شود.
    /// - باید: تراکنش‌ها همزمان ایجاد شوند.
    /// - نباید: انتقال ناقص انجام شود.
    /// - نباید: موجودی منفی ایجاد شود.
    /// </remarks>
    public WalletTransferResult TransferBetweenWallets(
        Wallet sourceWallet,
        Wallet destinationWallet,
        Money amount,
        string? referenceId = null,
        string? description = null,
        string? externalReference = null)
    {
        // Validate transfer
        var validation = ValidateWalletTransfer(sourceWallet, destinationWallet, amount);
        if (!validation.IsValid)
            throw new InvalidOperationException(validation.ErrorMessage);

        // Calculate transfer fee
        var feeAmount = CalculateTransferFee(amount, sourceWallet, destinationWallet);
        var totalAmount = feeAmount.AmountRials > 0 
            ? Money.FromRials(amount.AmountRials + feeAmount.AmountRials)
            : amount;

        // Check if source wallet has sufficient balance for amount + fee
        if (sourceWallet.Balance.IsLessThan(totalAmount))
            throw new InvalidOperationException("Insufficient balance for transfer including fees");

        // Perform transfer operations atomically
        var sourceTransaction = sourceWallet.TransferOut(
            totalAmount,
            destinationWallet.Id,
            referenceId,
            description,
            externalReference);

        var destinationTransaction = destinationWallet.TransferIn(
            amount, // Destination receives only the transfer amount, not the fee
            sourceWallet.Id,
            referenceId,
            description,
            externalReference);

        return new WalletTransferResult(
            sourceTransaction,
            destinationTransaction,
            amount,
            feeAmount.AmountRials > 0 ? feeAmount : null);
    }

    /// <summary>
    /// Validate if transfer is allowed between wallets
    /// </summary>
    public WalletTransferValidation ValidateWalletTransfer(
        Wallet sourceWallet,
        Wallet destinationWallet,
        Money amount)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Check source wallet status
        if (!sourceWallet.IsActive())
            errors.Add("Source wallet is not active");

        // Check destination wallet status
        if (!destinationWallet.IsActive())
            errors.Add("Destination wallet is not active");

        // Check if wallets are different
        if (sourceWallet.Id == destinationWallet.Id)
            errors.Add("Cannot transfer to the same wallet");

        // Check amount validity
        if (amount.AmountRials <= 0)
            errors.Add("Transfer amount must be positive");

        // Check sufficient balance
        var feeAmount = CalculateTransferFee(amount, sourceWallet, destinationWallet);
        var totalAmount = Money.FromRials(amount.AmountRials + feeAmount.AmountRials);
        
        if (!sourceWallet.HasSufficientBalance(totalAmount))
            errors.Add("Insufficient balance for transfer including fees");

        // Business rule validations
        if (amount.AmountRials > 100_000_000) // 100 million rials
            warnings.Add("Large transfer amount - additional verification may be required");

        // Check for suspicious patterns (simplified example)
        if (sourceWallet.NationalNumber == destinationWallet.NationalNumber)
            warnings.Add("Transfer between wallets of the same user");

        return new WalletTransferValidation(
            !errors.Any(),
            errors.FirstOrDefault(),
            warnings);
    }

    /// <summary>
    /// Calculate transfer fees based on business rules
    /// </summary>
    public Money CalculateTransferFee(Money amount, Wallet sourceWallet, Wallet destinationWallet)
    {
        // Business rules for transfer fees
        const decimal InternalTransferRate = 0.001m; // 0.1%
        const decimal ExternalTransferRate = 0.002m; // 0.2%
        const decimal MinFee = 1000m; // 1000 rials
        const decimal MaxFee = 100_000m; // 100,000 rials
        const decimal FreeTransferThreshold = 10_000m; // 10,000 rials

        // Free transfers for small amounts
        if (amount.AmountRials <= FreeTransferThreshold)
            return Money.Zero;

        // Determine if it's internal or external transfer
        var isInternalTransfer = sourceWallet.NationalNumber == destinationWallet.NationalNumber;
        var feeRate = isInternalTransfer ? InternalTransferRate : ExternalTransferRate;

        // Calculate fee
        var calculatedFee = amount.AmountRials * feeRate;
        
        // Apply minimum and maximum limits
        var finalFee = Math.Max(MinFee, Math.Min(MaxFee, calculatedFee));

        return Money.FromRials(finalFee);
    }

    #endregion

    #region Balance Operations

    /// <summary>
    /// Calculate available balance considering frozen amounts and pending transactions
    /// </summary>
    public WalletBalanceCalculation CalculateAvailableBalance(Wallet wallet)
    {
        var currentBalance = wallet.Balance;
        var warnings = new List<string>();

        // Extract frozen amount from metadata
        var frozenAmount = Money.Zero;
        if (wallet.Metadata.TryGetValue("FrozenAmount", out var frozenAmountStr) &&
            decimal.TryParse(frozenAmountStr, out var frozenValue))
        {
            frozenAmount = Money.FromRials(frozenValue);
        }

        // Calculate pending transactions (simplified - in real implementation, 
        // this would query pending transactions from repository)
        var pendingAmount = Money.Zero;
        var pendingTransactions = wallet.Transactions
            .Where(t => t.CreatedAt.Date == DateTime.UtcNow.Date)
            .Where(t => t.TransactionType == WalletTransactionType.Payment ||
                       t.TransactionType == WalletTransactionType.TransferOut)
            .Where(t => t.CreatedAt > DateTime.UtcNow.AddMinutes(-30)) // Recent transactions
            .Sum(t => t.Amount.AmountRials);

        if (pendingTransactions > 0)
        {
            pendingAmount = Money.FromRials(pendingTransactions);
            warnings.Add("Recent transactions may affect available balance");
        }

        // Calculate available balance
        var totalReserved = Money.FromRials(frozenAmount.AmountRials + pendingAmount.AmountRials);
        var availableBalance = currentBalance.AmountRials >= totalReserved.AmountRials
            ? Money.FromRials(currentBalance.AmountRials - totalReserved.AmountRials)
            : Money.Zero;

        // Add warnings for low balance
        if (availableBalance.AmountRials < 10_000) // Less than 10,000 rials
            warnings.Add("Low available balance");

        if (frozenAmount.AmountRials > 0)
            warnings.Add($"Amount frozen: {frozenAmount}");

        return new WalletBalanceCalculation(
            currentBalance,
            availableBalance,
            frozenAmount,
            pendingAmount,
            warnings);
    }

    /// <summary>
    /// Validate if wallet has sufficient balance for a transaction
    /// </summary>
    public WalletBalanceValidation ValidateSufficientBalance(
        Wallet wallet, 
        Money amount, 
        bool includePending = true)
    {
        var balanceCalculation = CalculateAvailableBalance(wallet);
        var availableAmount = includePending 
            ? balanceCalculation.AvailableBalance 
            : wallet.Balance;

        if (availableAmount.IsLessThan(amount))
        {
            return new WalletBalanceValidation(
                false,
                $"Insufficient balance. Required: {amount}, Available: {availableAmount}",
                amount,
                availableAmount);
        }

        return new WalletBalanceValidation(true, null, amount, availableAmount);
    }

    /// <summary>
    /// Calculate wallet balance history and trends
    /// </summary>
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

        // Calculate starting balance (simplified - would need historical data)
        var startingBalance = wallet.Balance;
        var totalInflow = Money.Zero;
        var totalOutflow = Money.Zero;

        foreach (var transaction in transactions)
        {
            if (transaction.IsDeposit())
                totalInflow = totalInflow.Add(transaction.Amount);
            else if (transaction.IsWithdrawal())
                totalOutflow = totalOutflow.Add(transaction.Amount);
        }

        // Calculate ending balance
        var endingBalance = Money.FromRials(
            startingBalance.AmountRials + totalInflow.AmountRials - totalOutflow.AmountRials);

        // Calculate daily trend points
        var trendPoints = new List<BalanceTrendPoint>();
        var dailyGroups = transactions.GroupBy(t => t.CreatedAt.Date).OrderBy(g => g.Key);
        
        var runningBalance = startingBalance;
        foreach (var dayGroup in dailyGroups)
        {
            var dailyInflow = Money.Zero;
            var dailyOutflow = Money.Zero;

            foreach (var transaction in dayGroup)
            {
                if (transaction.IsDeposit())
                    dailyInflow = dailyInflow.Add(transaction.Amount);
                else if (transaction.IsWithdrawal())
                    dailyOutflow = dailyOutflow.Add(transaction.Amount);
            }

            var dailyChange = Money.FromRials(dailyInflow.AmountRials - dailyOutflow.AmountRials);
            runningBalance = Money.FromRials(runningBalance.AmountRials + dailyChange.AmountRials);

            trendPoints.Add(new BalanceTrendPoint(
                dayGroup.Key,
                runningBalance,
                dailyChange));
        }

        return new WalletBalanceAnalysis(
            startingBalance,
            endingBalance,
            totalInflow,
            totalOutflow,
            transactions.Count,
            trendPoints);
    }

    #endregion

    #region Payment Operations

    /// <summary>
    /// Process bill payment using wallet balance
    /// </summary>
    public WalletPaymentResult ProcessBillPayment(
        Wallet wallet,
        Bill bill,
        Money amount,
        string? referenceId = null,
        string? description = null)
    {
        // Validate payment
        var validation = ValidateBillPayment(wallet, bill, amount);
        if (!validation.IsValid)
            throw new InvalidOperationException(validation.ErrorMessage);

        // Calculate payment details
        var paymentCalculation = CalculatePaymentDetails(wallet, bill, amount);
        var finalAmount = paymentCalculation.FinalAmount;

        // Check sufficient balance
        if (!wallet.HasSufficientBalance(finalAmount))
            throw new InvalidOperationException("Insufficient wallet balance");

        // Process wallet transaction
        var walletTransaction = wallet.PayBill(
            finalAmount,
            bill.Id,
            bill.BillNumber,
            referenceId,
            description);

        // Create bill payment
        var billPayment = bill.CreatePayment(
            finalAmount,
            PaymentMethod.Wallet,
            null, // No gateway for wallet payments
            null, // No callback URL needed
            description);

        return new WalletPaymentResult(
            walletTransaction,
            billPayment,
            amount,
            paymentCalculation.FeeAmount,
            paymentCalculation.DiscountAmount);
    }

    /// <summary>
    /// Validate if wallet payment is allowed for the bill
    /// </summary>
    public WalletPaymentValidation ValidateBillPayment(
        Wallet wallet,
        Bill bill,
        Money amount)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Check wallet status
        if (!wallet.IsActive())
            errors.Add("Wallet is not active");

        // Check bill status
        if (bill.Status == BillStatus.Cancelled)
            errors.Add("Cannot pay cancelled bill");
        if (bill.Status == BillStatus.FullyPaid)
            errors.Add("Bill is already fully paid");
        if (bill.Status == BillStatus.Voided)
            errors.Add("Cannot pay voided bill");

        // Check amount validity
        if (amount.AmountRials <= 0)
            errors.Add("Payment amount must be positive");
        if (amount.AmountRials > bill.RemainingAmount.AmountRials)
            errors.Add("Payment amount exceeds remaining bill amount");

        // Check wallet ownership
        if (wallet.NationalNumber != bill.UserNationalNumber)
            errors.Add("Wallet does not belong to bill owner");

        // Check sufficient balance
        var paymentCalculation = CalculatePaymentDetails(wallet, bill, amount);
        if (!wallet.HasSufficientBalance(paymentCalculation.FinalAmount))
            errors.Add("Insufficient wallet balance for payment including fees");

        // Business rule validations
        if (amount.AmountRials > 50_000_000) // 50 million rials
            warnings.Add("Large payment amount - additional verification may be required");

        if (bill.IsOverdue())
            warnings.Add("Bill is overdue - late fees may apply");

        return new WalletPaymentValidation(
            !errors.Any(),
            errors.FirstOrDefault(),
            warnings);
    }

    /// <summary>
    /// Calculate payment fees and discounts
    /// </summary>
    public PaymentCalculation CalculatePaymentDetails(
        Wallet wallet,
        Bill bill,
        Money amount)
    {
        var originalAmount = amount;
        var feeAmount = Money.Zero;
        var discountAmount = Money.Zero;
        var appliedDiscounts = new List<string>();

        // Calculate payment fee (simplified business rules)
        const decimal WalletPaymentFeeRate = 0.005m; // 0.5%
        const decimal MinFee = 500m; // 500 rials
        const decimal MaxFee = 50_000m; // 50,000 rials

        var calculatedFee = amount.AmountRials * WalletPaymentFeeRate;
        feeAmount = Money.FromRials(Math.Max(MinFee, Math.Min(MaxFee, calculatedFee)));

        // Apply discounts based on business rules
        // Early payment discount
        if (bill.DueDate.HasValue && DateTime.UtcNow < bill.DueDate.Value.AddDays(-7))
        {
            var earlyDiscount = Money.FromRials(amount.AmountRials * 0.02m); // 2% discount
            discountAmount = discountAmount.Add(earlyDiscount);
            appliedDiscounts.Add("Early payment discount (2%)");
        }

        // Large payment discount
        if (amount.AmountRials > 10_000_000) // More than 10 million rials
        {
            var largeDiscount = Money.FromRials(amount.AmountRials * 0.01m); // 1% discount
            discountAmount = discountAmount.Add(largeDiscount);
            appliedDiscounts.Add("Large payment discount (1%)");
        }

        // VIP customer discount (simplified - would check customer status)
        if (wallet.Metadata.ContainsKey("VIPCustomer") && 
            wallet.Metadata["VIPCustomer"] == "true")
        {
            var vipDiscount = Money.FromRials(amount.AmountRials * 0.005m); // 0.5% discount
            discountAmount = discountAmount.Add(vipDiscount);
            appliedDiscounts.Add("VIP customer discount (0.5%)");
        }

        // Calculate final amount
        var finalAmount = Money.FromRials(
            originalAmount.AmountRials + feeAmount.AmountRials - discountAmount.AmountRials);

        return new PaymentCalculation(
            originalAmount,
            finalAmount,
            feeAmount.AmountRials > 0 ? feeAmount : null,
            discountAmount.AmountRials > 0 ? discountAmount : null,
            appliedDiscounts);
    }

    /// <summary>
    /// Process refund to wallet from bill
    /// </summary>
    public WalletRefundResult ProcessRefundToWallet(
        Wallet wallet,
        Bill bill,
        Money refundAmount,
        string reason,
        string? referenceId = null)
    {
        // Validate refund
        if (!wallet.IsActive())
            throw new InvalidOperationException("Wallet is not active");
        if (bill.Status != BillStatus.FullyPaid && bill.Status != BillStatus.PartiallyPaid)
            throw new InvalidOperationException("Can only refund paid bills");
        if (refundAmount.AmountRials <= 0)
            throw new ArgumentException("Refund amount must be positive", nameof(refundAmount));
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Refund reason is required", nameof(reason));

        // Check refund amount doesn't exceed paid amount
        var totalRefunded = bill.Refunds
            .Where(r => r.Status == RefundStatus.Completed)
            .Sum(r => r.Amount.AmountRials);

        if (totalRefunded + refundAmount.AmountRials > bill.PaidAmount.AmountRials)
            throw new InvalidOperationException("Refund amount exceeds paid amount");

        // Create bill refund
        var billRefund = bill.CreateRefund(refundAmount, reason, wallet.NationalNumber);

        // Process wallet refund transaction
        var walletTransaction = wallet.ReceiveRefund(
            refundAmount,
            bill.Id,
            bill.BillNumber,
            referenceId,
            $"Refund from bill {bill.BillNumber}: {reason}");

        return new WalletRefundResult(
            walletTransaction,
            billRefund,
            refundAmount);
    }

    #endregion

    #region Transaction Limits

    /// <summary>
    /// Calculate daily/monthly transaction limits
    /// </summary>
    public TransactionLimitValidation ValidateTransactionLimits(
        Wallet wallet, 
        Money amount, 
        WalletTransactionType transactionType)
    {
        // Business rules for transaction limits
        var dailyLimit = Money.FromRials(50_000_000); // 50 million rials
        var monthlyLimit = Money.FromRials(500_000_000); // 500 million rials

        // Adjust limits based on transaction type
        switch (transactionType)
        {
            case WalletTransactionType.TransferOut:
                dailyLimit = Money.FromRials(100_000_000); // Higher limit for transfers
                break;
            case WalletTransactionType.Payment:
                dailyLimit = Money.FromRials(200_000_000); // Higher limit for payments
                break;
            case WalletTransactionType.Withdrawal:
                dailyLimit = Money.FromRials(20_000_000); // Lower limit for withdrawals
                break;
        }

        // Calculate used amounts for today and this month
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

        var usedTodayMoney = Money.FromRials(usedToday);
        var usedThisMonthMoney = Money.FromRials(usedThisMonth);

        // Check daily limit
        if (usedTodayMoney.Add(amount).IsGreaterThan(dailyLimit))
        {
            return new TransactionLimitValidation(
                false,
                $"Daily transaction limit exceeded. Used: {usedTodayMoney}, Limit: {dailyLimit}",
                dailyLimit,
                monthlyLimit,
                usedTodayMoney,
                usedThisMonthMoney);
        }

        // Check monthly limit
        if (usedThisMonthMoney.Add(amount).IsGreaterThan(monthlyLimit))
        {
            return new TransactionLimitValidation(
                false,
                $"Monthly transaction limit exceeded. Used: {usedThisMonthMoney}, Limit: {monthlyLimit}",
                dailyLimit,
                monthlyLimit,
                usedTodayMoney,
                usedThisMonthMoney);
        }

        return new TransactionLimitValidation(
            true,
            null,
            dailyLimit,
            monthlyLimit,
            usedTodayMoney,
            usedThisMonthMoney);
    }

    #endregion
}

#region Result Models

/// <summary>
/// Result of a wallet transfer operation
/// </summary>
public sealed record WalletTransferResult(
    WalletTransaction SourceTransaction,
    WalletTransaction DestinationTransaction,
    Money TransferAmount,
    Money? FeeAmount = null);

/// <summary>
/// Validation result for wallet transfer
/// </summary>
public sealed record WalletTransferValidation(
    bool IsValid,
    string? ErrorMessage = null,
    List<string>? Warnings = null);

/// <summary>
/// Result of wallet balance calculation
/// </summary>
public sealed record WalletBalanceCalculation(
    Money CurrentBalance,
    Money AvailableBalance,
    Money FrozenAmount,
    Money PendingAmount,
    List<string> Warnings);

/// <summary>
/// Result of balance validation
/// </summary>
public sealed record WalletBalanceValidation(
    bool IsValid,
    string? ErrorMessage = null,
    Money? RequiredAmount = null,
    Money? AvailableAmount = null);

/// <summary>
/// Result of balance analysis
/// </summary>
public sealed record WalletBalanceAnalysis(
    Money StartingBalance,
    Money EndingBalance,
    Money TotalInflow,
    Money TotalOutflow,
    int TransactionCount,
    List<BalanceTrendPoint> TrendPoints);

/// <summary>
/// Balance trend point for analysis
/// </summary>
public sealed record BalanceTrendPoint(
    DateTime Date,
    Money Balance,
    Money DailyChange);

/// <summary>
/// Result of wallet bill payment processing
/// </summary>
public sealed record WalletPaymentResult(
    WalletTransaction WalletTransaction,
    Payment BillPayment,
    Money PaymentAmount,
    Money? FeeAmount = null,
    Money? DiscountAmount = null);

/// <summary>
/// Validation result for wallet bill payment
/// </summary>
public sealed record WalletPaymentValidation(
    bool IsValid,
    string? ErrorMessage = null,
    List<string>? Warnings = null);

/// <summary>
/// Payment calculation details
/// </summary>
public sealed record PaymentCalculation(
    Money OriginalAmount,
    Money FinalAmount,
    Money? FeeAmount = null,
    Money? DiscountAmount = null,
    List<string>? AppliedDiscounts = null);

/// <summary>
/// Result of wallet refund processing
/// </summary>
public sealed record WalletRefundResult(
    WalletTransaction WalletTransaction,
    Refund BillRefund,
    Money RefundAmount);

/// <summary>
/// Result of transaction limit validation
/// </summary>
public sealed record TransactionLimitValidation(
    bool IsValid,
    string? ErrorMessage = null,
    Money? DailyLimit = null,
    Money? MonthlyLimit = null,
    Money? UsedToday = null,
    Money? UsedThisMonth = null);

#endregion
