using System;

namespace Nezam.Refahi.Domain.BoundedContexts.Payment.ValueObjects
{
  /// <summary>
  /// Represents the status of a dispute or chargeback process on a payment.
  /// This follows DDD value object pattern - representing a concept with no identity.
  /// </summary>
  public enum PaymentDisputeStatus
  {
    /// <summary>
    /// Dispute has been initiated and is currently open
    /// </summary>
    Open,

    /// <summary>
    /// Dispute has been resolved in merchant's favor
    /// </summary>
    ResolvedMerchantFavor,

    /// <summary>
    /// Dispute has been resolved in the customer's favor
    /// </summary>
    ResolvedCustomerFavor,

    /// <summary>
    /// Dispute is under review by the payment processor
    /// </summary>
    UnderReview,

    /// <summary>
    /// Merchant has responded to the dispute
    /// </summary>
    MerchantResponded,

    /// <summary>
    /// Dispute has been escalated to the payment processor or card network
    /// </summary>
    Escalated,

    /// <summary>
    /// Dispute has been canceled
    /// </summary>
    Canceled
  }
}
