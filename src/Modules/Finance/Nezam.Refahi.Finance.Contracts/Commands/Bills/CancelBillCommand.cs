using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Bills;

/// <summary>
/// Command to cancel a bill
/// </summary>
public record CancelBillCommand : IRequest<ApplicationResult<CancelBillResponse>>
{
    /// <summary>
    /// ID of the bill to cancel
    /// </summary>
    public Guid BillId { get; init; }

    /// <summary>
    /// Reason for cancellation (optional)
    /// </summary>
    public string? Reason { get; init; }
}

/// <summary>
/// Response for CancelBillCommand
/// </summary>
public record CancelBillResponse
{
    public Guid BillId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? CancellationReason { get; init; }
}