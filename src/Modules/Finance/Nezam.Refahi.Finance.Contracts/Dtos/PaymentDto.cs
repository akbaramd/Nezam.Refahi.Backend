using MCA.SharedKernel.Application.Dtos;
using Nezam.Refahi.Finance.Domain.Enums;

namespace Nezam.Refahi.Finance.Contracts.Dtos;

/// <summary>
/// Payment data transfer object
/// </summary>
public class PaymentDto : EntityDto<Guid>
{
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public long AmountRials { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentGateway? Gateway { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayReference { get; set; }
    public string? CallbackUrl { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? FailureReason { get; set; }

    // Summary information
    public bool IsExpired { get; set; }
    public int TransactionCount { get; set; }
    public string TrackingCode { get; set; } = string.Empty;

    // Related bill info
    public BillSummaryDto? Bill { get; set; }

    // Related data
    public List<PaymentTransactionDto> Transactions { get; set; } = new();
}

/// <summary>
/// Payment summary DTO for lists
/// </summary>
public class PaymentSummaryDto
{
    public Guid Id { get; set; }
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public long AmountRials { get; set; }
    public PaymentStatus Status { get; set; }
    public PaymentMethod Method { get; set; }
    public PaymentGateway? Gateway { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
}

/// <summary>
/// Payment creation request DTO
/// </summary>
public class CreatePaymentDto
{
    public Guid BillId { get; set; }
    public long AmountRials { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Online;
    public PaymentGateway? Gateway { get; set; }
    public string? CallbackUrl { get; set; }
    public string? Description { get; set; }
    public DateTime? ExpiryDate { get; set; }
}

/// <summary>
/// Payment gateway response DTO
/// </summary>
public class PaymentGatewayResponseDto
{
    public Guid PaymentId { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public string? GatewayTransactionId { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}