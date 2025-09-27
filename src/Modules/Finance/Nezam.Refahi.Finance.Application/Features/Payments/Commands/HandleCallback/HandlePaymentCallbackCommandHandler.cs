using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Finance.Application.Features.Payments.Commands.CompletePayment;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Shared.Domain.ValueObjects;
using Nezam.Refahi.Finance.Application.Configuration;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.HandleCallback;

/// <summary>
/// Handler for HandlePaymentCallbackCommand
/// Processes payment callbacks from payment gateways and updates payment entities
/// Follows SOLID principles - handles business logic, delegates gateway operations
/// </summary>
public class HandlePaymentCallbackCommandHandler : IRequestHandler<HandlePaymentCallbackCommand, ApplicationResult<PaymentCallbackResult>>
{
    private readonly IPaymentService _paymentService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBillRepository _billRepository;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<HandlePaymentCallbackCommandHandler> _logger;
    private readonly FrontendSettings _frontendSettings;

    public HandlePaymentCallbackCommandHandler(
        IPaymentService paymentService,
        IPaymentRepository paymentRepository,
        IBillRepository billRepository,
        IFinanceUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<HandlePaymentCallbackCommandHandler> logger,
        IOptions<FrontendSettings> frontendSettings)
    {
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _frontendSettings = frontendSettings?.Value ?? throw new ArgumentNullException(nameof(frontendSettings));
    }

    public async Task<ApplicationResult<PaymentCallbackResult>> Handle(
        HandlePaymentCallbackCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Processing payment callback command");

            // 1. Fetch callback data from gateway (gateway operation)
            var gatewayResult = await _paymentService.FetchCallbackAsync(cancellationToken);
            if (!gatewayResult.IsSuccess || gatewayResult.Data == null)
            {
                _logger.LogWarning("Failed to fetch callback from gateway - Errors: {Errors}",
                    string.Join(", ", gatewayResult.Errors ?? new List<string>()));
                
                // Try to get BillId from tracking number if available
                Guid? billId = null;
                if (gatewayResult.Data?.TrackingNumber != null)
                {
                    try
                    {
                        var paymentForError = await _paymentRepository.GetByTrackingNumberAsync(gatewayResult.Data.TrackingNumber.ToString(), cancellationToken);
                        billId = paymentForError?.BillId;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to lookup payment by tracking number for error case");
                    }
                }
                
                // Return error with redirect to failure page
                var redirectUrl = billId.HasValue 
                    ? _frontendSettings.GetPaymentFailureUrl(billId.Value)
                    : _frontendSettings.GetGenericPaymentFailureUrl();
                    
                return ApplicationResult<PaymentCallbackResult>.Success(new PaymentCallbackResult
                {
                    PaymentId = Guid.Empty,
                    BillId = billId ?? Guid.Empty,
                    TrackingNumber = gatewayResult.Data?.TrackingNumber ?? 0,
                    IsSuccessful = false,
                    Message = gatewayResult.Message ?? "خطا در دریافت اطلاعات از درگاه پرداخت",
                    TransactionCode = null,
                    Amount = Money.Zero,
                    ProcessedAt = DateTime.UtcNow,
                    NewStatus = PaymentStatus.Failed,
                    RedirectUrl = redirectUrl,
                    BillStatus = null,
                    BillTotalAmount = null,
                    BillPaidAmount = null,
                    BillRemainingAmount = null,
                    IsBillFullyPaid = false
                });
            }

            var callbackData = gatewayResult.Data;
            _logger.LogInformation("Gateway callback received - TrackingNumber: {TrackingNumber}, IsSuccessful: {IsSuccessful}",
                callbackData.TrackingNumber, callbackData.IsSuccessful);

            // 2. Find payment by tracking number (business logic)
            var payment = await _paymentRepository.GetByTrackingNumberAsync(callbackData.TrackingNumber.ToString(), cancellationToken);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for TrackingNumber: {TrackingNumber}", callbackData.TrackingNumber);
                
                // Return error with redirect to failure page
                return ApplicationResult<PaymentCallbackResult>.Success(new PaymentCallbackResult
                {
                    PaymentId = Guid.Empty,
                    BillId = Guid.Empty,
                    TrackingNumber = callbackData.TrackingNumber,
                    IsSuccessful = false,
                    Message = $"پرداخت مربوط به این شماره پیگیری یافت نشد: {callbackData.TrackingNumber}",
                    TransactionCode = null,
                    Amount = Money.Zero,
                    ProcessedAt = DateTime.UtcNow,
                    NewStatus = PaymentStatus.Failed,
                    RedirectUrl = "http://localhost:3000/payment/failed", // Generic failure page for payment not found
                    BillStatus = null,
                    BillTotalAmount = null,
                    BillPaidAmount = null,
                    BillRemainingAmount = null,
                    IsBillFullyPaid = false
                });
            }

            PaymentCallbackResult result;

            if (callbackData.IsSuccessful)
            {
                // 3. Verify payment with gateway (gateway operation)
                var verificationResult = await _paymentService.VerifyPaymentAsync(callbackData.TrackingNumber, cancellationToken);

                if (verificationResult.IsSuccess && verificationResult.Data != null && verificationResult.Data.IsSuccessful)
                {
                    _logger.LogInformation("Payment verified successfully - TrackingNumber: {TrackingNumber}, TransactionCode: {TransactionCode}",
                        callbackData.TrackingNumber, verificationResult.Data.TransactionId);

                    // 4. Complete payment using proper business logic
                    var completePaymentCommand = new CompletePaymentCommand
                    {
                        PaymentId = payment.Id,
                        GatewayTransactionId = verificationResult.Data.TransactionId ?? string.Empty,
                        GatewayReference = verificationResult.Data.Message
                    };

                    var completePaymentResult = await _mediator.Send(completePaymentCommand, cancellationToken);
                    
                    if (completePaymentResult.IsSuccess)
                    {
                        // Get updated bill and payment information
                        var updatedBill = await _billRepository.GetByIdAsync(payment.BillId, cancellationToken);
                        var updatedPayment = updatedBill?.Payments.FirstOrDefault(p => p.Id == payment.Id);

                        result = new PaymentCallbackResult
                        {
                            PaymentId = payment.Id,
                            BillId = payment.BillId,
                            BillNumber = updatedBill?.BillNumber ?? payment.BillNumber,
                            TrackingNumber = callbackData.TrackingNumber,
                            IsSuccessful = true,
                            Message = "پرداخت با موفقیت انجام شد",
                            TransactionCode = verificationResult.Data.TransactionId,
                            Amount = payment.Amount,
                            ProcessedAt = DateTime.UtcNow,
                            NewStatus = PaymentStatus.Completed,
                            RedirectUrl = _frontendSettings.GetPaymentSuccessUrl(payment.BillId),
                            // Bill completion information
                            BillStatus = updatedBill?.Status.ToString(),
                            BillTotalAmount = updatedBill?.TotalAmount.AmountRials,
                            BillPaidAmount = updatedBill?.PaidAmount.AmountRials,
                            BillRemainingAmount = updatedBill?.RemainingAmount.AmountRials,
                            IsBillFullyPaid = updatedBill?.Status == BillStatus.FullyPaid,
                            BillFullyPaidDate = updatedBill?.FullyPaidDate
                        };
                    }
                    else
                    {
                        _logger.LogError("Failed to complete payment - PaymentId: {PaymentId}, Errors: {Errors}",
                            payment.Id, string.Join(", ", completePaymentResult.Errors ?? new List<string>()));

                        result = new PaymentCallbackResult
                        {
                            PaymentId = payment.Id,
                            BillId = payment.BillId,
                            TrackingNumber = callbackData.TrackingNumber,
                            IsSuccessful = false,
                            Message = $"خطا در تکمیل پرداخت: {completePaymentResult.Message}",
                            TransactionCode = verificationResult.Data.TransactionId,
                            Amount = payment.Amount,
                            ProcessedAt = DateTime.UtcNow,
                            NewStatus = PaymentStatus.Failed,
                            RedirectUrl = _frontendSettings.GetPaymentFailureUrl(payment.BillId)
                        };
                    }
                }
                else
                {
                    _logger.LogWarning("Payment verification failed - TrackingNumber: {TrackingNumber}, Message: {Message}",
                        callbackData.TrackingNumber, verificationResult.Data?.Message);

                    // Mark payment as failed
                    payment.MarkAsFailed($"Gateway verification failed: {verificationResult.Data?.Message}");
                    await _paymentRepository.UpdateAsync(payment, cancellationToken: cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    result = new PaymentCallbackResult
                    {
                        PaymentId = payment.Id,
                        BillId = payment.BillId,
                        TrackingNumber = callbackData.TrackingNumber,
                        IsSuccessful = false,
                        Message = $"تایید پرداخت ناموفق: {verificationResult.Data?.Message}",
                        TransactionCode = verificationResult.Data?.TransactionId,
                        Amount = payment.Amount,
                        ProcessedAt = DateTime.UtcNow,
                        NewStatus = PaymentStatus.Failed,
                        RedirectUrl = _frontendSettings.GetPaymentFailureUrl(payment.BillId)
                    };
                }
            }
            else
            {
                _logger.LogWarning("Payment callback failed - TrackingNumber: {TrackingNumber}, Message: {Message}",
                    callbackData.TrackingNumber, callbackData.Message);

                // Mark payment as failed
                payment.MarkAsFailed($"Gateway callback failed: {callbackData.Message}");
                await _paymentRepository.UpdateAsync(payment, cancellationToken: cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Get bill information for response
                var bill = await _billRepository.GetByIdAsync(payment.BillId, cancellationToken);

                result = new PaymentCallbackResult
                {
                    PaymentId = payment.Id,
                    BillId = payment.BillId,
                    BillNumber = bill?.BillNumber ?? payment.BillNumber,
                    TrackingNumber = callbackData.TrackingNumber,
                    IsSuccessful = false,
                    Message = $"پرداخت ناموفق: {callbackData.Message}",
                    TransactionCode = null,
                    Amount = payment.Amount,
                    ProcessedAt = DateTime.UtcNow,
                    NewStatus = PaymentStatus.Failed,
                    RedirectUrl = _frontendSettings.GetPaymentFailureUrl(payment.BillId),
                    // Bill information even for failed payments
                    BillStatus = bill?.Status.ToString(),
                    BillTotalAmount = bill?.TotalAmount.AmountRials,
                    BillPaidAmount = bill?.PaidAmount.AmountRials,
                    BillRemainingAmount = bill?.RemainingAmount.AmountRials,
                    IsBillFullyPaid = bill?.Status == BillStatus.FullyPaid
                };
            }

            _logger.LogInformation("Payment callback processed - PaymentId: {PaymentId}, IsSuccessful: {IsSuccessful}",
                result.PaymentId, result.IsSuccessful);

            return ApplicationResult<PaymentCallbackResult>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while handling payment callback");
            
            // Try to get BillId from any available context (this is best effort)
            Guid? billId = null;
            try
            {
                // If we have access to the request context, try to extract BillId
                // This is a fallback attempt - may not always work
                var gatewayResult = await _paymentService.FetchCallbackAsync(cancellationToken);
                if (gatewayResult.IsSuccess && gatewayResult.Data?.TrackingNumber != null)
                {
                    var paymentForException = await _paymentRepository.GetByTrackingNumberAsync(gatewayResult.Data.TrackingNumber.ToString(), cancellationToken);
                    billId = paymentForException?.BillId;
                }
            }
            catch
            {
                // Ignore errors in this fallback attempt
            }
            
            // Return error with redirect to failure page
            var redirectUrl = billId.HasValue 
                ? _frontendSettings.GetPaymentFailureUrl(billId.Value)
                : _frontendSettings.GetGenericPaymentFailureUrl();
                
            return ApplicationResult<PaymentCallbackResult>.Success(new PaymentCallbackResult
            {
                PaymentId = Guid.Empty,
                BillId = billId ?? Guid.Empty,
                TrackingNumber = 0,
                IsSuccessful = false,
                Message = $"خطای سیستم: {ex.Message}",
                TransactionCode = null,
                Amount = Money.Zero,
                ProcessedAt = DateTime.UtcNow,
                NewStatus = PaymentStatus.Failed,
                RedirectUrl = redirectUrl,
                BillStatus = null,
                BillTotalAmount = null,
                BillPaidAmount = null,
                BillRemainingAmount = null,
                IsBillFullyPaid = false
            });
        }
    }
}
