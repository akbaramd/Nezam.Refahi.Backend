using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Bills;

/// <summary>
/// Command to issue a bill (finalize it and make it ready for payment)
/// </summary>
public record IssueBillCommand : IRequest<ApplicationResult<IssueBillResponse>>
{
    /// <summary>
    /// ID of the bill to issue
    /// </summary>
    public Guid BillId { get; init; }
}

/// <summary>
/// Response for IssueBillCommand
/// </summary>
public record IssueBillResponse
{
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public decimal TotalAmount { get; init; }
}