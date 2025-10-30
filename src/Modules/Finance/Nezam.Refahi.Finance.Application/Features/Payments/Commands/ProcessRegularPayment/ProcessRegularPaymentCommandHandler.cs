using MediatR;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.ProcessRegularPayment;

/// <summary>
/// Handler for ProcessRegularPaymentCommand - Handles regular payments with gateway processing
/// </summary>
public class ProcessRegularPaymentCommandHandler : IRequestHandler<ProcessRegularPaymentCommand, ApplicationResult<ProcessRegularPaymentResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IPaymentService _paymentService;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public ProcessRegularPaymentCommandHandler(
        IBillRepository billRepository,
        IPaymentService paymentService,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<ProcessRegularPaymentResponse>> Handle(
        ProcessRegularPaymentCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            // Get bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessRegularPaymentResponse>.Failure("Bill not found");
            }

            // Security check
            if (bill.ExternalUserId != request.ExternalUserId)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessRegularPaymentResponse>.Failure("Access denied: You can only process payments for your own bills");
            }

            // Validate bill is payable
            if (bill.Status == BillStatus.Draft)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessRegularPaymentResponse>.Failure("Cannot process payment for draft bill. Issue the bill first.");
            }

            if (bill.Status == BillStatus.Cancelled)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessRegularPaymentResponse>.Failure("Cannot process payment for cancelled bill.");
            }

            if (bill.Status == BillStatus.FullyPaid)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessRegularPaymentResponse>.Failure("Bill is already fully paid.");
            }

            // Validate payment amount
            if (request.AmountRials <= 0)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessRegularPaymentResponse>.Failure("Payment amount must be greater than zero.");
            }

            if (request.AmountRials > bill.RemainingAmount.AmountRials)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessRegularPaymentResponse>.Failure(
                    $"Payment amount ({request.AmountRials:N0} Rials) exceeds remaining bill amount ({bill.RemainingAmount.AmountRials:N0} Rials).");
            }

            // Parse payment method and gateway
            if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessRegularPaymentResponse>.Failure("Invalid payment method");
            }

            PaymentGateway? paymentGateway = null;
            if (!string.IsNullOrEmpty(request.PaymentGateway))
            {
                if (!Enum.TryParse<PaymentGateway>(request.PaymentGateway, true, out var gateway))
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<ProcessRegularPaymentResponse>.Failure("Invalid payment gateway");
                }
                paymentGateway = gateway;
            }

            // Validate gateway is provided for online payments
            if (paymentMethod == PaymentMethod.Online && paymentGateway == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessRegularPaymentResponse>.Failure("Payment gateway is required for online payments.");
            }

            // Create payment
            var amount = Money.FromRials(request.AmountRials);
            var payment = bill.CreatePayment(
                amount: amount,
                method: paymentMethod,
                gateway: paymentGateway,
                callbackUrl: request.CallbackUrl,
                description: request.Description,
                expiryDate: request.ExpiryDate
            );

            var trackingNumber = GenerateTrackingNumber();
            payment.SetGatewayRefreance(trackingNumber.ToString());
            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Process payment if online
            PaymentProcessingResult? paymentProcessingResult = null;
            if (paymentMethod == PaymentMethod.Online && !string.IsNullOrEmpty(request.CallbackUrl))
            {
                // Create gateway request
                var gatewayRequest = new PaymentGatewayRequest
                {
                    TrackingNumber = trackingNumber,
                    AmountRials = (long)request.AmountRials,
                    Gateway = paymentGateway!.Value.ToString(),
                    CallbackUrl = request.CallbackUrl,
                    Description = request.Description,
                    AdditionalData = new Dictionary<string, string>
                    {
                        ["PaymentId"] = payment.Id.ToString(),
                        ["BillId"] = bill.Id.ToString(),
                        ["BillNumber"] = bill.BillNumber
                    }
                };

                var processingResult = await _paymentService.ProcessPaymentAsync(
                    gatewayRequest, 
                    cancellationToken);

                if (processingResult.IsSuccess)
                {
                    paymentProcessingResult = processingResult.Data;
                }
                else
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<ProcessRegularPaymentResponse>.Failure(
                        processingResult.Errors?.FirstOrDefault() ?? "خطا در پردازش پرداخت آنلاین");
                }
            }

            // Prepare response
            var response = new ProcessRegularPaymentResponse
            {
                PaymentId = payment.Id,
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                Amount = payment.Amount.AmountRials,
                PaymentMethod = payment.Method.ToString(),
                Status = payment.Status.ToString(),
                CreatedAt = payment.CreatedAt,
                ExpiryDate = payment.ExpiryDate,
                GatewayRedirectUrl = paymentProcessingResult?.RedirectUrl,
                BillStatus = bill.Status.ToString(),
                BillTotalAmount = bill.TotalAmount.AmountRials,
                TrackingNumber = paymentProcessingResult?.TrackingNumber != null ? (long)paymentProcessingResult.TrackingNumber : trackingNumber,
                RequiresRedirect = paymentProcessingResult?.RedirectUrl != null,
                PaymentMessage = paymentProcessingResult?.Message,
                PaymentGateway = paymentGateway?.ToString()
            };

            var message = $"پرداخت با موفقیت ایجاد شد. مبلغ: {request.AmountRials:N0} ریال.";
            if (paymentProcessingResult?.RedirectUrl != null)
            {
                message += " لطفاً برای تکمیل پرداخت به درگاه پرداخت هدایت شوید.";
            }

            return ApplicationResult<ProcessRegularPaymentResponse>.Success(response, message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<ProcessRegularPaymentResponse>.Failure(ex, "Failed to process regular payment");
        }
    }

    /// <summary>
    /// Generates a unique tracking number for payment
    /// </summary>
    private long GenerateTrackingNumber()
    {
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var random = new Random().Next(1000, 9999);
        return timestamp * 10000 + random;
    }
}
