using System;
using System.Collections.Generic;

namespace Nezam.Refahi.Contracts.Finance.v1.Messages;

/// <summary>
/// Command message to instruct Finance module to create a bill.
/// </summary>
public class IssueBillCommandMessage
{
    // Correlation
    public string TrackingCode { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; } = Guid.Empty; // e.g., ReservationId
    public string ReferenceType { get; set; } = string.Empty; // entity name for correlation

    // User
    public Guid ExternalUserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;

    // Bill presentation
    public string BillTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Optional meta
    public Dictionary<string, string> Metadata { get; set; } = new();

    // Optional items
    public List<CreateBillItemMessage> Items { get; set; } = new();
}

public class CreateBillItemMessage
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal UnitPriceRials { get; set; }
    public int Quantity { get; set; }
    public decimal? DiscountPercentage { get; set; }
}



