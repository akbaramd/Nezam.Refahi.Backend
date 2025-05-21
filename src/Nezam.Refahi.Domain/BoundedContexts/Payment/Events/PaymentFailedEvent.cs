using System;
using Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.Events
{
    /// <summary>
    /// Domain event representing that a payment has failed.
    /// This event can be used for integration between bounded contexts.
    /// </summary>
    public class PaymentFailedEvent
    {
        public Guid PaymentId { get; }
        public Guid ReservationId { get; }
        public Guid? CustomerId { get; }
        public decimal Amount { get; }
        public string CurrencyCode { get; }
        public PaymentMethod PaymentMethod { get; }
        public string FailureReason { get; }
        public DateTimeOffset FailureDate { get; }

        public PaymentFailedEvent(
            Guid paymentId,
            Guid reservationId,
            Guid? customerId,
            decimal amount,
            string currencyCode,
            PaymentMethod paymentMethod,
            string failureReason,
            DateTimeOffset failureDate)
        {
            PaymentId = paymentId;
            ReservationId = reservationId;
            CustomerId = customerId;
            Amount = amount;
            CurrencyCode = currencyCode;
            PaymentMethod = paymentMethod;
            FailureReason = failureReason;
            FailureDate = failureDate;
        }
    }
}
