using System;
using System.Collections.Generic;

namespace Nezam.Refahi.Contracts.Finance.v1.Messages;

/// <summary>
/// Command message to mark a wallet deposit as AwaitingPayment after bill creation.
/// </summary>
public class MarkWalletDepositAwaitingPaymentCommandMessage
{
    public Guid WalletDepositId { get; set; }
    public string TrackingCode { get; set; } = string.Empty;

    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public decimal AmountRials { get; set; }
    public string Currency { get; set; } = "IRR";

    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;

    public Dictionary<string, string> Metadata { get; set; } = new();
}



