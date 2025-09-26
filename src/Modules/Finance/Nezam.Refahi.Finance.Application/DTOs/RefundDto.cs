namespace Nezam.Refahi.Finance.Application.DTOs;

/// <summary>
/// Refund data transfer object
/// </summary>
public class RefundDto
{
    public Guid Id { get; set; }
    public Guid BillId { get; set; }
    public long AmountRials { get; set; }
    public string Status { get; set; } = string.Empty; // RefundStatus as string
    public string Reason { get; set; } = string.Empty;
    public string RequestedByNationalNumber { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? GatewayRefundId { get; set; }
    public string? GatewayReference { get; set; }
    public string? ProcessorNotes { get; set; }
    public string? RejectionReason { get; set; }

    // Related bill info
    public BillSummaryDto? Bill { get; set; }
}

/// <summary>
/// Refund summary DTO for lists
/// </summary>
public class RefundSummaryDto
{
    public Guid Id { get; set; }
    public Guid BillId { get; set; }
    public long AmountRials { get; set; }
    public string Status { get; set; } = string.Empty; // RefundStatus as string
    public string Reason { get; set; } = string.Empty;
    public string RequestedByNationalNumber { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Refund request DTO
/// </summary>
public class CreateRefundDto
{
    public Guid BillId { get; set; }
    public long AmountRials { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? RequestedByNationalNumber { get; set; }
}

/// <summary>
/// Refund approval DTO
/// </summary>
public class ApproveRefundDto
{
    public Guid RefundId { get; set; }
    public string? ProcessorNotes { get; set; }
}

/// <summary>
/// Refund rejection DTO
/// </summary>
public class RejectRefundDto
{
    public Guid RefundId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}
