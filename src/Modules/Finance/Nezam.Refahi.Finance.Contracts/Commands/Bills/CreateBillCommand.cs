using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Bills;

/// <summary>
/// Command to create a new bill in draft status
/// </summary>
public record CreateBillCommand : IRequest<ApplicationResult<CreateBillResponse>>
{
    /// <summary>
    /// Title of the bill
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Reference identifier for tracking
    /// </summary>
    public string ReferenceId { get; init; } = string.Empty;

    /// <summary>
    /// Type of bill (e.g., "MembershipFee", "ServiceCharge")
    /// </summary>
    public string BillType { get; init; } = string.Empty;

    /// <summary>
    /// National number of the user
    /// </summary>
    public Guid ExternalUserId { get; init; }

    /// <summary>
    /// Full name of the user (optional)
    /// </summary>
    public string? UserFullName { get; init; }

    /// <summary>
    /// Description or notes for the bill
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Due date for payment (optional)
    /// </summary>
    public DateTime? DueDate { get; init; }

    /// <summary>
    /// Additional metadata for the bill
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// List of items to include in the bill
    /// </summary>
    public List<CreateBillItemRequest>? Items { get; init; }
}

/// <summary>
/// Request for creating a bill item
/// </summary>
public record CreateBillItemRequest
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal UnitPriceRials { get; init; }
    public int Quantity { get; init; } = 1;
    public decimal? DiscountPercentage { get; init; }
}

/// <summary>
/// Response for CreateBillCommand
/// </summary>
public record CreateBillResponse
{
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
}