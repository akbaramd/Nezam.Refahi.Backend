using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Parbad;
using Parbad.InvoiceBuilder;
using Parbad.Gateway.Mellat;
using Parbad.Gateway.ParbadVirtual;
using Parbad.Gateway.Parsian;
namespace Nezam.Refahi.Finance.Infrastructure.Services;

/// <summary>
/// Payment service focused only on gateway operations
/// Follows SOLID principles - Single Responsibility
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IOnlinePayment _onlinePayment;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IOnlinePayment onlinePayment,
        IHostEnvironment environment,
        ILogger<PaymentService> logger)
    {
        _onlinePayment = onlinePayment ?? throw new ArgumentNullException(nameof(onlinePayment));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<PaymentProcessingResult>> ProcessPaymentAsync(
        PaymentGatewayRequest request, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing payment gateway request - TrackingNumber: {TrackingNumber}, Amount: {Amount}, Gateway: {Gateway}",
                request.TrackingNumber, request.AmountRials, request.Gateway);

            // Generate proper callback URL if not provided or relative
            var finalCallbackUrl = GenerateCallbackUrl(request.CallbackUrl);

            // Prepare payment request using Parbad
            var payResult = await _onlinePayment.RequestAsync(invoice =>
            {
                invoice.SetTrackingNumber((long)request.TrackingNumber)
                    .SetAmount(10000)
                    .SetCallbackUrl(finalCallbackUrl);

                // Configure gateway based on environment and payment gateway
                ConfigureGateway(invoice, request.Gateway);

                
            });

            if (payResult == null)
            {
                _logger.LogError("Payment gateway request failed - TrackingNumber: {TrackingNumber}", request.TrackingNumber);
                return ApplicationResult<PaymentProcessingResult>.Failure("درخواست پرداخت با خطا مواجه شد");
            }

            _logger.LogInformation("Payment gateway request result - TrackingNumber: {TrackingNumber}, IsSucceed: {IsSucceed}, Status: {Status}, Message: {Message}",
                request.TrackingNumber, payResult.IsSucceed, payResult.Status, payResult.Message);

            if (!payResult.IsSucceed)
            {
                _logger.LogWarning("Payment gateway request failed - TrackingNumber: {TrackingNumber}, Message: {Message}",
                    request.TrackingNumber, payResult.Message);
                return ApplicationResult<PaymentProcessingResult>.Failure($"خطا در درخواست پرداخت: {payResult.Message}");
            }

            var result = new PaymentProcessingResult
            {
                TrackingNumber = request.TrackingNumber,
                RedirectUrl = GetRedirectUrl(payResult),
                IsSucceed = payResult.IsSucceed,
                Message = payResult.Message,
                Gateway = request.Gateway,
                AmountRials = request.AmountRials,
                RequestedAt = DateTime.UtcNow
            };

            return ApplicationResult<PaymentProcessingResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment gateway request for TrackingNumber: {TrackingNumber}", request.TrackingNumber);
            return ApplicationResult<PaymentProcessingResult>.Failure(ex, "خطا در پردازش درخواست پرداخت");
        }
    }

    public async Task<ApplicationResult<GatewayCallbackResult>> FetchCallbackAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching payment callback from gateway");

            // Let Parbad handle the callback
            var paymentResult = await _onlinePayment.FetchAsync();

            if (paymentResult == null)
            {
                _logger.LogError("Payment callback result is null");
                return ApplicationResult<GatewayCallbackResult>.Failure("نتیجه پرداخت دریافت نشد");
            }

            _logger.LogInformation("Payment callback received - TrackingNumber: {TrackingNumber}, IsSucceed: {IsSucceed}, Message: {Message}",
                paymentResult.TrackingNumber, paymentResult.IsSucceed, paymentResult.Message);

            var result = new GatewayCallbackResult
            {
                TrackingNumber = paymentResult.TrackingNumber,
                IsSuccessful = paymentResult.IsSucceed,
                Message = paymentResult.Message,
                GatewayName = paymentResult.GatewayName,
                AmountRials = (long)paymentResult.Amount.Value,
                ProcessedAt = DateTime.UtcNow,
                AdditionalData = new Dictionary<string, string>
                {
                    ["Status"] = paymentResult.Status.ToString(),
                    ["GatewayAccountName"] = paymentResult.GatewayAccountName ?? string.Empty
                }
            };

            return ApplicationResult<GatewayCallbackResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching payment callback");
            return ApplicationResult<GatewayCallbackResult>.Failure(ex, "خطا در دریافت نتیجه پرداخت");
        }
    }

    public async Task<ApplicationResult<PaymentVerificationResult>> VerifyPaymentAsync(
        long trackingNumber, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Verifying payment with gateway - TrackingNumber: {TrackingNumber}", trackingNumber);

            var verifyResult = await _onlinePayment.VerifyAsync(trackingNumber);

            if (verifyResult == null)
            {
                _logger.LogError("Payment verification result is null - TrackingNumber: {TrackingNumber}", trackingNumber);
                return ApplicationResult<PaymentVerificationResult>.Failure("خطا در تایید پرداخت");
            }

            _logger.LogInformation("Payment verification result - TrackingNumber: {TrackingNumber}, IsSucceed: {IsSucceed}, Status: {Status}",
                trackingNumber, verifyResult.IsSucceed, verifyResult.Status);

            var result = new PaymentVerificationResult
            {
                TrackingNumber = trackingNumber,
                IsVerified = true,
                IsSuccessful = verifyResult.IsSucceed,
                Message = verifyResult.Message,
                ReferenceNumber = verifyResult.TransactionCode,
                TransactionId = verifyResult.TransactionCode,
                AmountRials = (long)verifyResult.Amount.Value,
                VerifiedAt = DateTime.UtcNow
            };

            return ApplicationResult<PaymentVerificationResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying payment for TrackingNumber: {TrackingNumber}", trackingNumber);
            return ApplicationResult<PaymentVerificationResult>.Failure(ex, "خطا در تایید پرداخت");
        }
    }

    public async Task<IEnumerable<PaymentGatewayInfo>> GetSupportedGatewaysAsync()
    {
        await Task.CompletedTask; // For async interface compliance

        return new List<PaymentGatewayInfo>
        {
            new PaymentGatewayInfo
            {
                Gateway = PaymentGateway.Parsian.ToString(),
                Name = "Parsian",
                DisplayName = "بانک پارسیان",
                IsEnabled = true,
                SupportsDevelopmentMode = true,
                MinAmount = 1000,
                MaxAmount = 500_000_000
            },
            new PaymentGatewayInfo
            {
                Gateway = PaymentGateway.Mellat.ToString(),
                Name = "Mellat",
                DisplayName = "بانک ملت",
                IsEnabled = true,
                SupportsDevelopmentMode = true,
                MinAmount = 1000,
                MaxAmount = 500_000_000
            },
            new PaymentGatewayInfo
            {
                Gateway = PaymentGateway.Zarinpal.ToString(),
                Name = "Zarinpal",
                DisplayName = "زرین‌پال",
                IsEnabled = true,
                SupportsDevelopmentMode = true,
                MinAmount = 1000,
                MaxAmount = 500_000_000
            }
        };
    }

    private void ConfigureGateway(IInvoiceBuilder invoice, string gateway)
    {
        if (_environment.IsDevelopment())
        {
            invoice.UseParbadVirtual();
            _logger.LogInformation("Using Parbad Virtual gateway in development mode");
        }
        else
        {
            switch (gateway)
            {
                case "Parsian":
                    invoice.UseParsian();
                    break;
                case "Mellat":
                    invoice.UseMellat();
                    break;
                case "Zarinpal":
                    // invoice.UseZarinPal(); // Not available in current Parbad version
                    invoice.UseParsian(); // Use Parsian as fallback
                    break;
                default:
                    invoice.UseParsian(); // Default to Parsian
                    break;
            }

            _logger.LogInformation("Using {Gateway} gateway in production mode", gateway);
        }
    }

    private string? GetRedirectUrl(IPaymentRequestResult payResult)
    {
        return payResult.GatewayTransporter?.Descriptor?.Url;
    }

    private string GenerateCallbackUrl(string? providedCallbackUrl)
    {
        // If a full URL is provided, use it as is
        if (!string.IsNullOrEmpty(providedCallbackUrl) && 
            (providedCallbackUrl.StartsWith("http://") || providedCallbackUrl.StartsWith("https://")))
        {
            return providedCallbackUrl;
        }

        // Generate default callback URL
        var baseUrl = _environment.IsDevelopment() 
            ? "https://localhost:5001" // Default development URL
            : "https://api.nezam-refahi.ir"; // Production URL - this should be configurable

        return $"{baseUrl}/api/v1/payments/callback";
    }
}