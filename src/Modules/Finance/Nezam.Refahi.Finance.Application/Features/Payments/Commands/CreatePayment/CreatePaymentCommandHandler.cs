using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.CreatePayment;

/// <summary>
/// Handler for CreatePaymentCommand - Creates payments with comprehensive bill management
///
/// Complete Business Logic & Accounting Operations:
///
/// 1. BILL MANAGEMENT:
///    - Adds additional items to draft bills before payment
///    - Automatically calculates item totals with quantity and discounts
///    - Recalculates bill total amount after item additions
///    - Auto-issues draft bills if AutoIssueBill is enabled
///    - Validates bill status transitions (Draft → Issued → Payable)
///
/// 2. PAYMENT PROCESSING:
///    - Creates payment attempts with multiple gateway support
///    - Validates payment amounts against bill remaining balance
///    - Supports partial and full payments
///    - Handles payment method validation (Online, Cash, Card)
///    - Sets appropriate payment expiry for online transactions
///
/// 3. ACCOUNTING OPERATIONS:
///    - Updates accounts receivable when bills are issued
///    - Creates payment intention records for audit trail
///    - Maintains accurate bill balance calculations
///    - Tracks payment attempts and their lifecycle
///    - Supports revenue recognition through bill issuance
///
/// 4. BUSINESS RULES:
///    - Cannot create payments for cancelled bills
///    - Cannot overpay bills (payment amount ≤ remaining amount)
///    - Only issued bills are payable (auto-issue available)
///    - Maintains referential integrity between bills and payments
///    - Enforces payment gateway requirements for online payments
///
/// 5. STATUS MANAGEMENT:
///    - Bill Status: Draft → Issued → PartiallyPaid → FullyPaid
///    - Payment Status: Pending → Processing → Completed/Failed/Cancelled
///    - Automatic status transitions based on business events
///    - Maintains consistent state across all entities
/// </summary>
public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApplicationResult<CreatePaymentResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentService _paymentService;
    private readonly IValidator<CreatePaymentCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(
        IBillRepository billRepository,
        IPaymentRepository paymentRepository,
        IPaymentService paymentService,
        IValidator<CreatePaymentCommand> validator,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<CreatePaymentResponse>> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            // Validate request
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<CreatePaymentResponse>.Failure(errors, "Validation failed");
            }

            // Get bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure("Bill not found");
            }

            // Track business operations for response
            int itemsAdded = 0;
            bool billWasIssued = false;

 
            // STEP 2: AUTO-ISSUE BILL IF REQUIRED
            if (bill.Status == BillStatus.Draft && request.AutoIssueBill)
            {
                if (!bill.Items.Any())
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreatePaymentResponse>.Failure(
                        "Cannot issue empty bill. Add at least one item to the bill before creating payment.");
                }

                bill.Issue();
                billWasIssued = true;
            }

            // STEP 3: VALIDATE BILL IS PAYABLE
            if (bill.Status == BillStatus.Draft)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure(
                    "Cannot create payment for draft bill. Set AutoIssueBill to true or issue the bill manually.");
            }

            if (bill.Status == BillStatus.Cancelled)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure(
                    "Cannot create payment for cancelled bill.");
            }

            if (bill.Status == BillStatus.FullyPaid)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure(
                    "Bill is already fully paid. No additional payment needed.");
            }

            // STEP 4: VALIDATE PAYMENT AMOUNT
            if (request.AmountRials <= 0)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure(
                    "Payment amount must be greater than zero.");
            }

            if (request.AmountRials > bill.RemainingAmount.AmountRials)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure(
                    $"Payment amount ({request.AmountRials:N0} Rials) exceeds remaining bill amount ({bill.RemainingAmount.AmountRials:N0} Rials).");
            }

            // STEP 5: VALIDATE PAYMENT METHOD AND GATEWAY
            if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var paymentMethod))
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure("Invalid payment method");
            }

            PaymentGateway? paymentGateway = null;
            if (!string.IsNullOrEmpty(request.PaymentGateway))
            {
                if (!Enum.TryParse<PaymentGateway>(request.PaymentGateway, true, out var gateway))
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreatePaymentResponse>.Failure("Invalid payment gateway");
                }
                paymentGateway = gateway;
            }

            // Validate gateway is provided for online payments
            if (paymentMethod == PaymentMethod.Online && paymentGateway == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure(
                    "Payment gateway is required for online payments.");
            }

            // STEP 6: CREATE PAYMENT
            var amount = Money.FromRials(request.AmountRials);
            var payment = bill.CreatePayment(
                amount: amount,
                method: paymentMethod,
                gateway: paymentGateway,
                callbackUrl: request.CallbackUrl,
                description: request.Description,
                expiryDate: request.ExpiryDate
            );
            var traclingNumber = GenerateTrackingNumber();
            payment.SetTrackingNumber(traclingNumber.ToString());
            // STEP 7: SAVE ALL CHANGES
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // STEP 8: PROCESS PAYMENT IF ONLINE
            PaymentProcessingResult? paymentProcessingResult = null;
            if (paymentMethod == PaymentMethod.Online && !string.IsNullOrEmpty(request.CallbackUrl))
            {
            
                // Create gateway request (gateway-focused)
                var gatewayRequest = new PaymentGatewayRequest
                {
                    TrackingNumber = traclingNumber,
                    AmountRials = (long)amount.AmountRials,
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
                    
                    // Update payment with tracking number (business logic)
                    // Note: Payment entity should have a method to set tracking number
                    // payment.SetTrackingNumber(gatewayRequest.TrackingNumber.ToString());
                    // await _paymentRepository.UpdateAsync(payment, cancellationToken: cancellationToken);
                    // await _unitOfWork.SaveAsync(cancellationToken);
                }
                else
                {
                    // Log the error but don't fail the entire operation
                    // The payment was created successfully, just the gateway processing failed
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreatePaymentResponse>.Failure(
                        processingResult.Errors?.FirstOrDefault() ?? "خطا در پردازش پرداخت آنلاین");
                }
            }

            // STEP 9: PREPARE COMPREHENSIVE RESPONSE
            var response = new CreatePaymentResponse
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
                ItemsAdded = itemsAdded,
                BillWasIssued = billWasIssued,
                TrackingNumber = paymentProcessingResult?.TrackingNumber != null ? (long)paymentProcessingResult.TrackingNumber : null,
                RequiresRedirect = paymentProcessingResult?.RedirectUrl != null,
                PaymentMessage = paymentProcessingResult?.Message,
                PaymentGateway = paymentGateway?.ToString()
            };

            var successMessage = $"Payment created successfully. ";
            if (itemsAdded > 0)
                successMessage += $"{itemsAdded} item(s) added to bill. ";
            if (billWasIssued)
                successMessage += "Bill was automatically issued. ";

            return ApplicationResult<CreatePaymentResponse>.Success(response, successMessage.Trim());
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CreatePaymentResponse>.Failure(ex, "Failed to create payment");
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
