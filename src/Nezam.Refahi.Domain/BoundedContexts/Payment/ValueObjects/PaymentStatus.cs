namespace Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects
{
    /// <summary>
    /// Represents the status of a payment transaction in various stages of the payment lifecycle.
    /// Following DDD principles, this value object represents a distinct concept in the domain
    /// with well-defined state transitions.
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// The payment has been initialized but not yet processed
        /// </summary>
        Pending,
        
        /// <summary>
        /// The payment has been authorized but not yet captured (funds on hold)
        /// </summary>
        Authorized,
        
        /// <summary>
        /// The payment has been fully captured and completed successfully
        /// </summary>
        Completed,
        
        /// <summary>
        /// The payment was partially captured (less than authorized amount)
        /// </summary>
        PartiallyCompleted,
        
        /// <summary>
        /// The payment has failed during processing
        /// </summary>
        Failed,
        
        /// <summary>
        /// The payment was fully refunded
        /// </summary>
        Refunded,
        
        /// <summary>
        /// The payment was partially refunded
        /// </summary>
        PartiallyRefunded,
        
        /// <summary>
        /// The payment was canceled before processing
        /// </summary>
        Canceled,
        
        /// <summary>
        /// The payment is under dispute (chargeback initiated)
        /// </summary>
        Disputed,
        
        /// <summary>
        /// The payment was charged back (dispute resolved in customer's favor)
        /// </summary>
        ChargedBack,
        
        /// <summary>
        /// The payment is being verified (additional security checks)
        /// </summary>
        Verifying,
        
        /// <summary>
        /// The payment was rejected due to fraud detection
        /// </summary>
        FraudRejected,
        
        /// <summary>
        /// The payment authorization has expired before capture
        /// </summary>
        AuthorizationExpired
    }
}
