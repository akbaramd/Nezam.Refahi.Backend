using MediatR;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Commands.Payments;

/// <summary>
/// Command to apply discount code to a bill
/// </summary>
public record ApplyDiscountCodeCommand : IRequest<ApplicationResult<ApplyDiscountCodeResponse>>
{
    public Guid BillId { get; init; }
    public string DiscountCode { get; init; } = string.Empty;
    public Guid ExternalUserId { get; init; }
}

/// <summary>
/// Response for discount code application
/// </summary>
public record ApplyDiscountCodeResponse
{
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string AppliedDiscountCode { get; init; } = string.Empty;
    public decimal AppliedDiscountAmount { get; init; }
    public decimal OriginalBillAmount { get; init; }
    public decimal FinalBillAmount { get; init; }
    public bool IsFreeBill { get; init; }
    public string Status { get; init; } = string.Empty;
}
