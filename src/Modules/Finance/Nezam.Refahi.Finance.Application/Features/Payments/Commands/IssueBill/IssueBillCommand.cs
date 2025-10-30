using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Payments;

/// <summary>
/// Command to issue a draft bill
/// </summary>
public record IssueBillCommand : IRequest<ApplicationResult<IssueBillResponse>>
{
    public Guid BillId { get; init; }
    public Guid ExternalUserId { get; init; }
}

/// <summary>
/// Response for bill issuance
/// </summary>
public record IssueBillResponse
{
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public bool IsFreeBill { get; init; }
}
