using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Bills;

/// <summary>
/// Command to remove an item from a bill
/// </summary>
public record RemoveBillItemCommand : IRequest<ApplicationResult<RemoveBillItemResponse>>
{
    /// <summary>
    /// ID of the bill
    /// </summary>
    public Guid BillId { get; init; }

    /// <summary>
    /// ID of the item to remove
    /// </summary>
    public Guid ItemId { get; init; }
}

/// <summary>
/// Response for RemoveBillItemCommand
/// </summary>
public record RemoveBillItemResponse
{
    public Guid BillId { get; init; }
    public decimal TotalBillAmount { get; init; }
}