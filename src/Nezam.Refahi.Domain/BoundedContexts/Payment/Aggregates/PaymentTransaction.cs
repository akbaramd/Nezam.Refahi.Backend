using System;
using Nezam.Refahi.Domain.BoundedContexts.Accommodation.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared;
using Nezam.Refahi.Domain.BoundedContexts.Shared.Entities;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.Aggregates
{
    /// <summary>
    /// Represents a payment transaction, which is the root aggregate of the Payment bounded context.
    /// Following DDD principles, this encapsulates all payment-related behaviors and enforces business invariants.
    /// </summary>
    public class PaymentTransaction : BaseEntity
    {
        public Guid ReservationId { get; private set; }
        public Guid? CustomerId { get; private set; }
        public Money Amount { get; private set; } = Money.Zero;
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string? TransactionReference { get; private set; }
        public string? AuthorizationCode { get; private set; }
        public string? CaptureReference { get; private set; }
        public string? Gateway { get; private set; }
        public DateTimeOffset TransactionDate { get; private set; }
        public DateTimeOffset? CompletionDate { get; private set; }
        public string? FailureReason { get; private set; }
        public string? ReceiptNumber { get; private set; }
        public bool IsRefund { get; private set; }
        public Guid? RefundedTransactionId { get; private set; }
        public string? RefundReason { get; private set; }
        public Money? RefundFee { get; private set; }
        public Money? OriginalAmount { get; private set; }
        public decimal? ExchangeRate { get; private set; }
        public string? OriginalCurrency { get; private set; }
        
        // For partial captures
        public Money? AuthorizedAmount { get; private set; }
        public Money? CapturedAmount { get; private set; }
        
        // For payment disputes
        public PaymentDisputeStatus? DisputeStatus { get; private set; }
        public string? DisputeReason { get; private set; }
        public DateTimeOffset? DisputeDate { get; private set; }
        public DateTimeOffset? DisputeResolutionDate { get; private set; }
        
        // For recurring payments/installments
        public Guid? InstallmentPlanId { get; private set; }
        public int? InstallmentNumber { get; private set; }
        public int? TotalInstallments { get; private set; }
        
        // For handling retry attempts
        public Guid? PreviousAttemptId { get; private set; }
        public int AttemptNumber { get; private set; } = 1;
        
        // Required by EF Core
        private PaymentTransaction() : base() { }

        public PaymentTransaction(
            Guid id,
            Guid reservationId,
            Guid? customerId,
            Money amount,
            PaymentMethod paymentMethod,
            string? gateway,
            bool isRefund = false,
            Guid? refundedTransactionId = null)
        {
            if (amount == null)
                throw new ArgumentNullException(nameof(amount), "Payment amount cannot be null");
            
            if (amount.Amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero", nameof(amount));
            
            if (isRefund && !refundedTransactionId.HasValue)
                throw new ArgumentException("Refunded transaction ID is required for refund payments", nameof(refundedTransactionId));
            Id = id;
            ReservationId = reservationId;
            CustomerId = customerId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Status = PaymentStatus.Pending;
            TransactionDate = DateTimeOffset.UtcNow;
            Gateway = gateway;
            IsRefund = isRefund;
            RefundedTransactionId = refundedTransactionId;
        }

        public void SetTransactionReference(string transactionReference)
        {
            if (string.IsNullOrWhiteSpace(transactionReference))
                throw new ArgumentException("Transaction reference cannot be empty", nameof(transactionReference));
            
            TransactionReference = transactionReference;
            UpdateModifiedAt();
        }

        public void MarkAsCompleted(string? receiptNumber = null)
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Payment is already completed");
            
            if (Status == PaymentStatus.Failed)
                throw new InvalidOperationException("Failed payment cannot be marked as completed");
            
            if (Status == PaymentStatus.Refunded)
                throw new InvalidOperationException("Refunded payment cannot be marked as completed");
            
            if (Status == PaymentStatus.Disputed || Status == PaymentStatus.ChargedBack)
                throw new InvalidOperationException("Disputed or charged back payment cannot be marked as completed");
            
            Status = PaymentStatus.Completed;
            CompletionDate = DateTimeOffset.UtcNow;
            ReceiptNumber = receiptNumber;
            UpdateModifiedAt();
        }

        public void MarkAsFailed(string failureReason)
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Completed payment cannot be marked as failed");
            
            if (Status == PaymentStatus.Failed)
                throw new InvalidOperationException("Payment is already marked as failed");
            
            if (string.IsNullOrWhiteSpace(failureReason))
                throw new ArgumentException("Failure reason cannot be empty", nameof(failureReason));
            
            Status = PaymentStatus.Failed;
            FailureReason = failureReason;
            UpdateModifiedAt();
        }

        public void MarkAsPending()
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Completed payment cannot be marked as pending");
            
            Status = PaymentStatus.Pending;
            UpdateModifiedAt();
        }

        public bool CanRefund()
        {
            return Status == PaymentStatus.Completed && !IsRefund;
        }
        
        #region New Advanced Payment Features
        
        /// <summary>
        /// Sets the authorization code and marks the payment as authorized but not yet captured.
        /// </summary>
        public void MarkAsAuthorized(string authorizationCode)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be authorized");
                
            if (string.IsNullOrWhiteSpace(authorizationCode))
                throw new ArgumentException("Authorization code cannot be empty", nameof(authorizationCode));
            
            AuthorizationCode = authorizationCode;
            Status = PaymentStatus.Authorized;
            AuthorizedAmount = Amount; // Store the full authorized amount
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Captures an authorized payment, either fully or partially.
        /// </summary>
        public void MarkAsCaptured(Money captureAmount, string? captureReference = null)
        {
            if (Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Only authorized payments can be captured");
                
            if (captureAmount == null)
                throw new ArgumentNullException(nameof(captureAmount));
                
            if (AuthorizedAmount == null)
                throw new InvalidOperationException("No authorized amount recorded for this transaction");
            
            if (captureAmount.Amount > AuthorizedAmount.Amount)
                throw new ArgumentException("Capture amount cannot exceed authorized amount", nameof(captureAmount));

            CapturedAmount = captureAmount;
            CaptureReference = captureReference;
            
            // If capturing the full amount, mark as completed, otherwise partially completed
            if (captureAmount.Amount == AuthorizedAmount.Amount)
            {
                Status = PaymentStatus.Completed;
                CompletionDate = DateTimeOffset.UtcNow;
            }
            else
            {
                Status = PaymentStatus.PartiallyCompleted;
            }
            
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Records currency exchange information for cross-currency payments.
        /// </summary>
        public void RecordCurrencyExchange(Money localAmount, decimal exchangeRate)
        {
            if (Status != PaymentStatus.Pending && Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Currency exchange can only be recorded for pending or authorized payments");
            
            OriginalAmount = Amount;
            // Store the currency code as a string from the original amount
            OriginalCurrency = Amount.Currency; // Changed from CurrencyCode to Currency
            ExchangeRate = exchangeRate;
            
            // The amount becomes the converted local currency amount
            Amount = localAmount;
            
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Marks a payment as part of an installment plan.
        /// </summary>
        public void LinkToInstallmentPlan(Guid installmentPlanId, int installmentNumber, int totalInstallments)
        {
            if (InstallmentPlanId.HasValue)
                throw new InvalidOperationException("Payment is already linked to an installment plan");
                
            if (installmentNumber <= 0)
                throw new ArgumentException("Installment number must be positive", nameof(installmentNumber));
                
            if (totalInstallments <= 0)
                throw new ArgumentException("Total installments must be positive", nameof(totalInstallments));
                
            if (installmentNumber > totalInstallments)
                throw new ArgumentException("Installment number cannot exceed total installments");
            
            InstallmentPlanId = installmentPlanId;
            InstallmentNumber = installmentNumber;
            TotalInstallments = totalInstallments;
            
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Links this payment to a previous failed attempt.
        /// </summary>
        public void LinkToPreviousAttempt(Guid previousAttemptId, int attemptNumber = 0)
        {
            if (PreviousAttemptId.HasValue)
                throw new InvalidOperationException("Payment is already linked to a previous attempt");
            
            PreviousAttemptId = previousAttemptId;
            
            if (attemptNumber > 1)
            {
                AttemptNumber = attemptNumber;
            }
            else
            {
                AttemptNumber = 2; // Default to second attempt if not specified
            }
            
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Marks a payment as disputed and records dispute details.
        /// </summary>
        public void MarkAsDisputed(string reason)
        {
            if (Status != PaymentStatus.Completed && Status != PaymentStatus.PartiallyCompleted)
                throw new InvalidOperationException("Only completed payments can be disputed");
                
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Dispute reason cannot be empty", nameof(reason));
            
            Status = PaymentStatus.Disputed;
            DisputeStatus = PaymentDisputeStatus.Open;
            DisputeReason = reason;
            DisputeDate = DateTimeOffset.UtcNow;
            
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Resolves a payment dispute with the specified outcome.
        /// </summary>
        public void ResolveDispute(bool inMerchantFavor, string resolution)
        {
            if (Status != PaymentStatus.Disputed)
                throw new InvalidOperationException("Only disputed payments can be resolved");
                
            if (DisputeStatus != PaymentDisputeStatus.Open)
                throw new InvalidOperationException("Only open disputes can be resolved");
                
            if (string.IsNullOrWhiteSpace(resolution))
                throw new ArgumentException("Resolution details cannot be empty", nameof(resolution));
            
            DisputeResolutionDate = DateTimeOffset.UtcNow;
            
            if (inMerchantFavor)
            {
                DisputeStatus = PaymentDisputeStatus.ResolvedMerchantFavor;
                Status = PaymentStatus.Completed; // Restore to completed status
            }
            else
            {
                DisputeStatus = PaymentDisputeStatus.ResolvedCustomerFavor;
                Status = PaymentStatus.ChargedBack;
            }
            
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Sets the reason for a refund.
        /// </summary>
        public void SetRefundReason(string? reason)
        {
            if (!IsRefund)
                throw new InvalidOperationException("Can only set refund reason on refund transactions");
                
            RefundReason = reason;
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Applies a fee to a refund transaction.
        /// </summary>
        public void ApplyRefundFee(Money fee)
        {
            if (!IsRefund)
                throw new InvalidOperationException("Can only apply refund fee on refund transactions");
                
            if (fee == null)
                throw new ArgumentNullException(nameof(fee));
                
            if (fee.Amount <= 0)
                throw new ArgumentException("Refund fee must be greater than zero", nameof(fee));
                
            // Ensure fee doesn't exceed refund amount
            if (fee.Amount >= Amount.Amount)
                throw new ArgumentException("Refund fee cannot exceed or equal refund amount", nameof(fee));
            
            RefundFee = fee;
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Marks a payment as being verified for fraud or security reasons.
        /// </summary>
        public void MarkAsVerifying(string reason)
        {
            if (Status != PaymentStatus.Pending && Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Only pending or authorized payments can be marked for verification");
            
            Status = PaymentStatus.Verifying;
            FailureReason = $"Under verification: {reason}";
            
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Marks a payment as rejected due to fraud detection.
        /// </summary>
        public void MarkAsFraudRejected(string reason)
        {
            if (Status == PaymentStatus.Completed)
                throw new InvalidOperationException("Completed payments cannot be marked as fraud rejected");
                
            Status = PaymentStatus.FraudRejected;
            FailureReason = $"Fraud rejection: {reason}";
            
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Marks an authorized payment as expired.
        /// </summary>
        public void MarkAuthorizationAsExpired()
        {
            if (Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Only authorized payments can expire");
            
            Status = PaymentStatus.AuthorizationExpired;
            UpdateModifiedAt();
        }
        
        /// <summary>
        /// Checks if a partial refund is possible and in what amount.
        /// </summary>
        public Money GetMaxRefundableAmount()
        {
            if (!CanRefund())
                throw new InvalidOperationException("This transaction cannot be refunded");
            
            // Get previously refunded amount for this transaction
            // This would typically involve querying the repository, but for illustration:
            decimal previouslyRefunded = 0; // This would need to be calculated
            
            decimal maxRefundable = Amount.Amount - previouslyRefunded;
            
            if (maxRefundable <= 0)
                throw new InvalidOperationException("No refundable amount remains");
                
            return new Money(maxRefundable, Amount.Currency); // Changed from CurrencyCode to Currency
        }
        
        #endregion
    }
}
