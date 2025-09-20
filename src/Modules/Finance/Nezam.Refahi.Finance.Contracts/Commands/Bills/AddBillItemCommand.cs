using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Bills;

/// <summary>
/// Command to add an item to a bill
/// </summary>
public record AddBillItemCommand : IRequest<ApplicationResult<AddBillItemResponse>>
{
    /// <summary>
    /// ID of the bill to add item to
    /// </summary>
    public Guid BillId { get; init; }

    /// <summary>
    /// Title of the bill item
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Description of the item (optional)
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Unit price in Rials
    /// </summary>
    public decimal UnitPriceRials { get; init; }

    /// <summary>
    /// Quantity of the item
    /// </summary>
    public int Quantity { get; init; } = 1;

    /// <summary>
    /// Discount percentage (optional, 0-100)
    /// </summary>
    public decimal? DiscountPercentage { get; init; }
}

/// <summary>
/// Response for AddBillItemCommand
/// </summary>
public record AddBillItemResponse
{
    public Guid BillId { get; init; }
    public Guid ItemId { get; init; }
    public decimal TotalBillAmount { get; init; }
}