using System;

namespace Nezam.Refahi.Contracts.Finance.v1.Messages;

/// <summary>
/// Command message to mark a wallet deposit as Failed and trigger rollback actions.
/// </summary>
public class FailWalletDepositCommandMessage
{
    public Guid WalletDepositId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;

    public string FailureStage { get; set; } = string.Empty; // e.g., Requested, Completion
    public string FailureReason { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }

    // Optional context
    public Guid? BillId { get; set; }
    public string? BillNumber { get; set; }
    public Guid? PaymentId { get; set; }
}



