using MediatR;
using Nezam.Refahi.Finance.Contracts.Dtos;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Contracts.Queries.Wallets;

/// <summary>
/// Query to get wallet deposits for a user
/// </summary>
public class GetWalletDepositsQuery : IRequest<ApplicationResult<WalletDepositsResponse>>
{
    public string UserNationalNumber { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

/// <summary>
/// Response for GetWalletDepositsQuery
/// </summary>
public class WalletDepositsResponse
{
    public string UserNationalNumber { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<WalletDepositDto> Deposits { get; set; } = new();
}

/// <summary>
/// DTO for wallet deposit
/// </summary>
public class WalletDepositDto
{
    public Guid DepositId { get; set; }
    public Guid WalletId { get; set; }
    public Guid? BillId { get; set; }
    public string UserNationalNumber { get; set; } = string.Empty;
    public long AmountRials { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ExternalReference { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? BillNumber { get; set; }
    public Dictionary<string, string> Metadata { get; set; } = new();
}
