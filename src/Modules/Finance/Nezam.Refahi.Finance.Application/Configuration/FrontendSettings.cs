namespace Nezam.Refahi.Finance.Application.Configuration;

/// <summary>
/// Configuration settings for frontend URLs
/// </summary>
public class FrontendSettings
{
    public const string SectionName = "FrontendSettings";

    /// <summary>
    /// Base URL for the frontend application
    /// Default: http://localhost:3000
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:3000";

    /// <summary>
    /// Payment success page path
    /// Default: /bill/{billId}/payment/success
    /// </summary>
    public string PaymentSuccessPath { get; set; } = "/bills/{trackingCode}/payments/success/{paymentId}?billType={billType}";

    /// <summary>
    /// Payment failure page path
    /// Default: /bill/{billId}/payment/failed
    /// </summary>
    public string PaymentFailurePath { get; set; } = "/bills/{trackingCode}/payments/failed/{paymentId}?billType={billType}";
    public string GenericPaymentFailurePath { get; set; } = "/bills/payment/failed";
    /// <summary>
    /// Generic payment failure page path (when no BillId available)
    /// Default: /payment/failed

    /// <summary>
    /// Builds the complete payment success URL
    /// </summary>
    public string GetPaymentSuccessUrl(Guid billId,Guid paymentId,string? trackingCode,string? billType)
    {
        return $"{BaseUrl.TrimEnd('/')}{PaymentSuccessPath
          .Replace("{billId}", billId.ToString())
          .Replace("{trackingCode}", trackingCode??"error")
          .Replace("{billType}", billType??"error")
          .Replace("{paymentId}", paymentId.ToString())}";
    }

    /// <summary>
    /// Builds the complete payment failure URL
    /// </summary>
    public string GetPaymentFailureUrl(Guid billId,Guid paymentId,string? trackingCode,string? billType)
    {
        return $"{BaseUrl.TrimEnd('/')}{PaymentFailurePath
          .Replace("{billId}", billId.ToString())
          .Replace("{trackingCode}", trackingCode??"error")
          .Replace("{billType}", billType??"error")
          .Replace("{paymentId}", paymentId.ToString())}";
    }


}
