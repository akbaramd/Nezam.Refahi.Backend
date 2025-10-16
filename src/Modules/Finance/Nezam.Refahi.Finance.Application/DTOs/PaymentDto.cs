using MCA.SharedKernel.Application.Dtos;

namespace Nezam.Refahi.Finance.Application.DTOs;

/// <summary>
/// Payment data transfer object
/// </summary>
public class PaymentDto : EntityDto<Guid>
{
    public Guid BillId { get; set; }
    public string BillNumber { get; set; } = string.Empty;
    public decimal AmountRials { get; set; }
    public string Status { get; set; } = string.Empty; // PaymentStatus as string
    public string Method { get; set; } = string.Empty; // PaymentMethod as string
    public string? Gateway { get; set; } // PaymentGateway as string
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
    public decimal AmountRials { get; set; }
    public string Status { get; set; } = string.Empty; // PaymentStatus as string
    public string Method { get; set; } = string.Empty; // PaymentMethod as string
    public string? Gateway { get; set; } // PaymentGateway as string
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
    public decimal AmountRials { get; set; }
    public string Method { get; set; } = "Online"; // PaymentMethod as string, default Online
    public string? Gateway { get; set; } // PaymentGateway as string
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
