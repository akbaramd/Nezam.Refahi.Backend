using MediatR;
using Nezam.Refahi.Finance.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Commands.Bills;

/// <summary>
/// Command to create a bill for charging a wallet
/// </summary>
public record CreateBillChargeBillCommand : IRequest<ApplicationResult<CreateBillChargeBillResponse>>
{
    /// <summary>
    /// User's national number
    /// </summary>
    public string UserNationalNumber { get; init; } = string.Empty;

    /// <summary>
    /// User's full name
    /// </summary>
    public string UserFullName { get; init; } = string.Empty;

    /// <summary>
    /// Amount to charge in rials
    /// </summary>
    public decimal AmountRials { get; init; }

    /// <summary>
    /// Description for the charge bill
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// External reference (e.g., bank transaction ID)
    /// </summary>
    public string? ExternalReference { get; init; }

    /// <summary>
    /// Additional metadata for the bill
    /// </summary>
    public Dictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Response for CreateBillChargeBillCommand
/// </summary>
public record CreateBillChargeBillResponse
{
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string UserNationalNumber { get; init; } = string.Empty;
    public string UserFullName { get; init; } = string.Empty;
    public decimal AmountRials { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime IssueDate { get; init; }
    public string ReferenceId { get; init; } = string.Empty;
    public Guid DepositId { get; init; }
}
