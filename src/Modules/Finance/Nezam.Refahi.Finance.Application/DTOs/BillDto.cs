using MCA.SharedKernel.Application.Dtos;

namespace Nezam.Refahi.Finance.Application.DTOs;

/// <summary>
/// Bill data transfer object
/// </summary>
public class BillDto : DeletableAggregateDto<Guid>
{
    public string BillNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string BillType { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public string? UserFullName { get; set; }
    public string Status { get; set; } = string.Empty; // BillStatus as string
    public long TotalAmountRials { get; set; }
    public long PaidAmountRials { get; set; }
    public long RemainingAmountRials { get; set; }
    public string? Description { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? FullyPaidDate { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();

    // Summary information
    public bool IsOverdue { get; set; }
    public decimal PaymentCompletionPercentage { get; set; }
    public int ItemCount { get; set; }
    public int PaymentCount { get; set; }
    public int RefundCount { get; set; }

    // Related data
    public List<BillItemDto> Items { get; set; } = new();
    public List<PaymentSummaryDto> Payments { get; set; } = new();
    public List<RefundSummaryDto> Refunds { get; set; } = new();
}

/// <summary>
/// Bill summary DTO for lists
/// </summary>
public class BillSummaryDto
{
    public Guid Id { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string BillType { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public string? UserFullName { get; set; }
    public string Status { get; set; } = string.Empty; // BillStatus as string
    public long TotalAmountRials { get; set; }
    public long PaidAmountRials { get; set; }
    public long RemainingAmountRials { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsOverdue { get; set; }
    public decimal PaymentCompletionPercentage { get; set; }
}

/// <summary>
/// Bill creation request DTO
/// </summary>
public class CreateBillDto
{
    public string Title { get; set; } = string.Empty;
    public string ReferenceId { get; set; } = string.Empty;
    public string BillType { get; set; } = string.Empty;
    public Guid ExternalUserId { get; set; }
    public string? UserFullName { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public List<CreateBillItemDto> Items { get; set; } = new();
}

/// <summary>
/// Bill update request DTO
/// </summary>
public class UpdateBillDto
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Bill statistics DTO
/// </summary>
public class BillStatisticsDto
{
    public Dictionary<string, int> StatusCounts { get; set; } = new(); // BillStatus as string keys
    public Dictionary<string, int> TypeCounts { get; set; } = new();
    public long TotalAmountRials { get; set; }
    public long PaidAmountRials { get; set; }
    public long PendingAmountRials { get; set; }
    public int OverdueBillsCount { get; set; }
}
