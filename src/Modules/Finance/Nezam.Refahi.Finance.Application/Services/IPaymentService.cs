using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Services;

/// <summary>
/// Service for handling payment gateway operations only
/// Does not handle payment entity operations - follows SOLID principles
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Processes payment request with gateway and returns redirect information
    /// </summary>
    /// <param name="request">Payment gateway request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment processing result with redirect URL</returns>
    Task<ApplicationResult<PaymentProcessingResult>> ProcessPaymentAsync(
        PaymentGatewayRequest request, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches payment result from gateway callback
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Gateway callback result</returns>
    Task<ApplicationResult<GatewayCallbackResult>> FetchCallbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies payment with gateway using tracking number
    /// </summary>
    /// <param name="trackingNumber">Payment tracking number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Payment verification result from gateway</returns>
    Task<ApplicationResult<PaymentVerificationResult>> VerifyPaymentAsync(
        long trackingNumber, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets supported payment gateways
    /// </summary>
    /// <returns>List of supported gateways</returns>
    Task<IEnumerable<PaymentGatewayInfo>> GetSupportedGatewaysAsync();
}

/// <summary>
/// Request for payment gateway processing
/// </summary>
public class PaymentGatewayRequest
{
    public decimal TrackingNumber { get; set; }
    public decimal AmountRials { get; set; } // Using primitive long instead of Money value object
    public string Gateway { get; set; } = string.Empty; // PaymentGateway as string
    public string CallbackUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = new();
}

/// <summary>
/// Result of payment processing from gateway
/// </summary>
public class PaymentProcessingResult
{
    public decimal TrackingNumber { get; set; }
    public string? RedirectUrl { get; set; }
    public bool IsSucceed { get; set; }
    public string? Message { get; set; }
    public string Gateway { get; set; } = string.Empty; // PaymentGateway as string
    public decimal AmountRials { get; set; } // Using primitive long instead of Money value object
    public DateTime RequestedAt { get; set; }
}

/// <summary>
/// Result of gateway callback
/// </summary>
public class GatewayCallbackResult
{
    public decimal TrackingNumber { get; set; }
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
    public string? GatewayName { get; set; }
    public decimal AmountRials { get; set; } // Using primitive long instead of Money value object
    public DateTime ProcessedAt { get; set; }
    public Dictionary<string, string> AdditionalData { get; set; } = new();
}

/// <summary>
/// Result of payment verification from gateway
/// </summary>
public class PaymentVerificationResult
{
    public decimal TrackingNumber { get; set; }
    public bool IsVerified { get; set; }
    public bool IsSuccessful { get; set; }
    public string? Message { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? TransactionId { get; set; }
    public decimal AmountRials { get; set; } // Using primitive long instead of Money value object
    public DateTime VerifiedAt { get; set; }
}

/// <summary>
/// Information about payment gateway
/// </summary>
public class PaymentGatewayInfo
{
    public string Gateway { get; set; } = string.Empty; // PaymentGateway as string
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public bool SupportsDevelopmentMode { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
}
