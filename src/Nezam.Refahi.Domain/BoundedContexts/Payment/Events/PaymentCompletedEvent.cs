using System;
using Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects;
using Nezam.Refahi.Domain.BoundedContexts.Shared;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.Events
{
    /// <summary>
    /// Domain event representing that a payment has been completed.
    /// This event can be used for integration between bounded contexts.
    /// </summary>
    public class PaymentCompletedEvent
    {
        public Guid PaymentId { get; }
        public Guid ReservationId { get; }
        public Guid? CustomerId { get; }
        public decimal Amount { get; }
        public string CurrencyCode { get; }
        public PaymentMethod PaymentMethod { get; }
        public string? TransactionReference { get; }
        public DateTimeOffset CompletionDate { get; }
        public string? ReceiptNumber { get; }
        public bool IsRefund { get; }
        public Guid? RefundedTransactionId { get; }

        public PaymentCompletedEvent(
            Guid paymentId,
            Guid reservationId,
            Guid? customerId,
            decimal amount,
            string currencyCode,
            PaymentMethod paymentMethod,
            DateTimeOffset completionDate,
            string? transactionReference = null,
            string? receiptNumber = null,
            bool isRefund = false,
            Guid? refundedTransactionId = null)
        {
            PaymentId = paymentId;
            ReservationId = reservationId;
            CustomerId = customerId;
            Amount = amount;
            CurrencyCode = currencyCode;
            PaymentMethod = paymentMethod;
            CompletionDate = completionDate;
            TransactionReference = transactionReference;
            ReceiptNumber = receiptNumber;
            IsRefund = isRefund;
            RefundedTransactionId = refundedTransactionId;
        }
    }
}
