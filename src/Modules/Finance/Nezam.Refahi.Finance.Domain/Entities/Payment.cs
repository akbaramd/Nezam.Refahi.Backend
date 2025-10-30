using MCA.SharedKernel.Domain;
using MCA.SharedKernel.Domain.AggregateRoots;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Events;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Domain.Entities;

/// <summary>
/// پرداخت – ثبت هر تراکنش مالی مربوط به یک صورت‌حساب.
/// </summary>
public sealed class Payment : SoftDeletableAggregateRoot<Guid>
{
    public Guid BillId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentGateway? Gateway { get; private set; }

    public string? GatewayTransactionId { get; private set; }
    public string? GatewayReference { get; private set; }

    public string? Description { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }

    // تخفیف‌ها
    public string? AppliedDiscountCode { get; private set; }
    public Guid? AppliedDiscountCodeId { get; private set; }
    public Money? AppliedDiscountAmount { get; private set; }
    public bool IsFreePayment { get; private set; }

    // ارتباطات
    public Bill Bill { get; private set; } = null!;

    private readonly List<PaymentTransaction> _transactions = new();
    public IReadOnlyCollection<PaymentTransaction> Transactions => _transactions.AsReadOnly();

    private Payment() : base() { }

    public Payment(
        Guid billId,
        Money amount,
        PaymentMethod method = PaymentMethod.Online,
        PaymentGateway? gateway = null,
        string? description = null,
        DateTime? expiryDate = null)
        : base(Guid.NewGuid())
    {
        if (billId == Guid.Empty)
            throw new ArgumentException("Bill ID cannot be empty.", nameof(billId));
        if (amount == null || amount.AmountRials <= 0)
            throw new ArgumentException("Payment amount must be greater than zero.", nameof(amount));

        BillId = billId;
        Amount = amount;
        Method = method;
        Gateway = gateway;
        Description = description?.Trim();
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        ExpiryDate = expiryDate ?? DateTime.UtcNow.AddMinutes(15);
    }

    public void SetProcessing(string? gatewayTransactionId = null)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can be set to processing.");

        Status = PaymentStatus.Processing;
        GatewayTransactionId = gatewayTransactionId?.Trim();

        AddDomainEvent(new PaymentProcessingEvent(
            Id,
            BillId,
            Amount,
            Gateway,
            GatewayTransactionId));
    }

    public void MarkAsCompleted(string? gatewayTransactionId = null)
    {
        if (Status != PaymentStatus.Processing && Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only processing or pending payments can be completed.");

        Status = Method == PaymentMethod.Wallet
            ? PaymentStatus.PaidFromWallet
            : PaymentStatus.Completed;

        GatewayTransactionId = gatewayTransactionId?.Trim();
        CompletedAt = DateTime.UtcNow;
        ExpiryDate = null;
    }

    public void MarkAsFailed(string? reason = null)
    {
        if (Status is PaymentStatus.Completed or PaymentStatus.PaidFromWallet)
            throw new InvalidOperationException("Cannot mark completed or wallet payments as failed.");

        Status = PaymentStatus.Failed;
        FailureReason = reason?.Trim();
    }

    public void Cancel(string? reason = null)
    {
        if (Status is PaymentStatus.Completed or PaymentStatus.PaidFromWallet)
            throw new InvalidOperationException("Cannot cancel completed payments.");
        if (Status == PaymentStatus.Cancelled)
            return;

        Status = PaymentStatus.Cancelled;
        FailureReason = reason?.Trim();
    }

    public void MarkAsExpired()
    {
        if (Status is PaymentStatus.Completed or PaymentStatus.PaidFromWallet)
            throw new InvalidOperationException("Cannot expire completed payments.");
        if (Status != PaymentStatus.Pending)
            return;

        Status = PaymentStatus.Expired;

        AddDomainEvent(new PaymentExpiredEvent(
            Id,
            BillId,
            Amount,
            Method,
            Gateway,
            ExpiryDate));
    }

    public void MarkAsRefunded(string? reason = null)
    {
        if (Status is not (PaymentStatus.Completed or PaymentStatus.PaidFromWallet))
            throw new InvalidOperationException("Only completed or wallet payments can be refunded.");

        Status = PaymentStatus.Refunded;
        FailureReason = reason?.Trim();
    }

    public bool IsExpired()
    {
        return Status == PaymentStatus.Pending &&
               ExpiryDate.HasValue &&
               DateTime.UtcNow > ExpiryDate.Value;
    }

    public void SetDiscount(string? code, Guid? codeId, Money? amount, bool isFree = false)
    {
      AppliedDiscountCode = code?.Trim();
      AppliedDiscountCodeId = codeId;
      AppliedDiscountAmount = amount;
        IsFreePayment = isFree;
    }

    public void SetGatewayRefreance(string toString)
    {
      GatewayReference = toString;
    }
}
