using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Features.Payments.Commands.ApplyDiscountCode;
using Nezam.Refahi.Finance.Application.Features.Payments.Commands.IssueBill;
using Nezam.Refahi.Finance.Application.Features.Payments.Commands.ProcessFreePayment;
using Nezam.Refahi.Finance.Application.Features.Payments.Commands.ProcessRegularPayment;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using MassTransit;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.CreatePayment;

/// <summary>
/// Simplified CreatePaymentCommandHandler - Orchestrates smaller, focused commands
/// 
/// Responsibilities:
/// 1. Validates the request
/// 2. Applies discount code if provided
/// 3. Issues bill if required
/// 4. Routes to appropriate payment processor based on bill amount
/// 5. Returns comprehensive response
/// </summary>
public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApplicationResult<CreatePaymentResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IValidator<CreatePaymentCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly IBus _publishEndpoint;

    public CreatePaymentCommandHandler(
        IBillRepository billRepository,
        IValidator<CreatePaymentCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        IMediator mediator,
        IBus publishEndpoint)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
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

            // Security check
            if (bill.ExternalUserId != request.ExternalUserId)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure("Access denied: You can only create payments for your own bills");
            }

            // Track operations for response
            bool billWasIssued = false;
            string? appliedDiscountCode = null;
            decimal appliedDiscountAmount = 0;
            decimal originalBillAmount = bill.TotalAmount.AmountRials;
            bool isFreeBill = false;

            // STEP 1: APPLY DISCOUNT CODE IF PROVIDED
            if (!string.IsNullOrEmpty(request.DiscountCode))
            {
                var applyDiscountCommand = new ApplyDiscountCodeCommand
                {
                    BillId = request.BillId,
                    DiscountCode = request.DiscountCode,
                    ExternalUserId = request.ExternalUserId
                };

                var discountResult = await _mediator.Send(applyDiscountCommand, cancellationToken);
                if (!discountResult.IsSuccess)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreatePaymentResponse>.Failure(discountResult.Errors?.FirstOrDefault() ?? "Failed to apply discount code");
                }

                appliedDiscountCode = discountResult.Data?.AppliedDiscountCode;
                appliedDiscountAmount = discountResult.Data?.AppliedDiscountAmount ?? 0;
                isFreeBill = discountResult.Data?.IsFreeBill ?? false;

                // Get updated bill
                bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            }

            // STEP 2: ISSUE BILL IF REQUIRED
            if (bill.Status == BillStatus.Draft && request.AutoIssueBill)
            {
                if (!bill.Items.Any())
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreatePaymentResponse>.Failure(
                        "Cannot issue empty bill. Add at least one item to the bill before creating payment.");
                }

                var issueBillCommand = new IssueBillCommand
                {
                    BillId = request.BillId,
                    ExternalUserId = request.ExternalUserId
                };

                var issueResult = await _mediator.Send(issueBillCommand, cancellationToken);
                if (!issueResult.IsSuccess)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreatePaymentResponse>.Failure(issueResult.Errors?.FirstOrDefault() ?? "Failed to issue bill");
                }

                billWasIssued = true;
                isFreeBill = issueResult.Data?.IsFreeBill ?? false;

                // Get updated bill
                bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
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
                return ApplicationResult<CreatePaymentResponse>.Failure("Cannot create payment for cancelled bill.");
            }

            if (bill.Status == BillStatus.FullyPaid)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreatePaymentResponse>.Failure("Bill is already fully paid.");
            }

            // STEP 4: ROUTE TO APPROPRIATE PAYMENT PROCESSOR
            CreatePaymentResponse response;
            string successMessage;

            if (bill.RemainingAmount.AmountRials <= 0 || isFreeBill)
            {
                // Process free payment
                var freePaymentCommand = new ProcessFreePaymentCommand
                {
                    BillId = request.BillId,
                    ExternalUserId = request.ExternalUserId,
                    Description = request.Description
                };

                var freeResult = await _mediator.Send(freePaymentCommand, cancellationToken);
                if (!freeResult.IsSuccess)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreatePaymentResponse>.Failure(freeResult.Errors?.FirstOrDefault() ?? "Failed to process free payment");
                }

                response = MapFreePaymentResponse(freeResult.Data!, bill, billWasIssued, appliedDiscountCode, appliedDiscountAmount, originalBillAmount);
                successMessage = "پرداخت رایگان با موفقیت پردازش شد.";
            }
            else
            {
                // Process regular payment
                var regularPaymentCommand = new ProcessRegularPaymentCommand
                {
                    BillId = request.BillId,
                    AmountRials = request.AmountRials,
                    PaymentMethod = request.PaymentMethod,
                    PaymentGateway = request.PaymentGateway,
                    CallbackUrl = request.CallbackUrl,
                    Description = request.Description,
                    ExpiryDate = request.ExpiryDate,
                    ExternalUserId = request.ExternalUserId
                };

                var regularResult = await _mediator.Send(regularPaymentCommand, cancellationToken);
                if (!regularResult.IsSuccess)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return ApplicationResult<CreatePaymentResponse>.Failure(regularResult.Errors?.FirstOrDefault() ?? "Failed to process regular payment");
                }

                response = MapRegularPaymentResponse(regularResult.Data!, bill, billWasIssued, appliedDiscountCode, appliedDiscountAmount, originalBillAmount);
                successMessage = "پرداخت با موفقیت ایجاد شد.";
            }

            // Add additional context to success message
            if (billWasIssued)
                successMessage += " صورتحساب به صورت خودکار صادر شد.";
            if (!string.IsNullOrEmpty(appliedDiscountCode))
                successMessage += $" کد تخفیف '{appliedDiscountCode}' اعمال شد.";

            // STEP 5: PUBLISH INTEGRATION EVENTS AND COMMIT TRANSACTION
            // Get the created payment from the response
            var createdPayment = bill.Payments.FirstOrDefault(p => p.Id == response.PaymentId);
           

            // Save changes (including Outbox messages) and commit transaction
            await _unitOfWork.CommitAsync(cancellationToken);

            return ApplicationResult<CreatePaymentResponse>.Success(response, successMessage);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CreatePaymentResponse>.Failure(ex, "Failed to create payment");
        }
    }

    private CreatePaymentResponse MapFreePaymentResponse(
        ProcessFreePaymentResponse freeResponse,
        Bill bill,
        bool billWasIssued,
        string? appliedDiscountCode,
        decimal appliedDiscountAmount,
        decimal originalBillAmount)
    {
        return new CreatePaymentResponse
        {
            PaymentId = freeResponse.PaymentId,
            BillId = freeResponse.BillId,
            BillNumber = freeResponse.BillNumber,
            Amount = freeResponse.Amount,
            PaymentMethod = freeResponse.PaymentMethod,
            Status = freeResponse.Status,
            CreatedAt = freeResponse.CreatedAt,
            ExpiryDate = null,
            GatewayRedirectUrl = null,
            BillStatus = freeResponse.BillStatus,
            BillTotalAmount = freeResponse.BillTotalAmount,
            ItemsAdded = 0,
            BillWasIssued = billWasIssued,
            TrackingNumber = null,
            RequiresRedirect = false,
            PaymentMessage = freeResponse.PaymentStatus,
            PaymentGateway = null,
            AppliedDiscountCode = appliedDiscountCode,
            AppliedDiscountAmount = appliedDiscountAmount,
            OriginalBillAmount = originalBillAmount,
            FinalBillAmount = bill.TotalAmount.AmountRials,
            IsFreePayment = true,
            PaymentSkipped = true,
            PaymentStatus = "پرداخت رایگان - تکمیل شد"
        };
    }

    private CreatePaymentResponse MapRegularPaymentResponse(
        ProcessRegularPaymentResponse regularResponse,
        Bill bill,
        bool billWasIssued,
        string? appliedDiscountCode,
        decimal appliedDiscountAmount,
        decimal originalBillAmount)
    {
        return new CreatePaymentResponse
        {
            PaymentId = regularResponse.PaymentId,
            BillId = regularResponse.BillId,
            BillNumber = regularResponse.BillNumber,
            Amount = regularResponse.Amount,
            PaymentMethod = regularResponse.PaymentMethod,
            Status = regularResponse.Status,
            CreatedAt = regularResponse.CreatedAt,
            ExpiryDate = regularResponse.ExpiryDate,
            GatewayRedirectUrl = regularResponse.GatewayRedirectUrl,
            BillStatus = regularResponse.BillStatus,
            BillTotalAmount = regularResponse.BillTotalAmount,
            ItemsAdded = 0,
            BillWasIssued = billWasIssued,
            TrackingNumber = regularResponse.TrackingNumber,
            RequiresRedirect = regularResponse.RequiresRedirect,
            PaymentMessage = regularResponse.PaymentMessage,
            PaymentGateway = regularResponse.PaymentGateway,
            AppliedDiscountCode = appliedDiscountCode,
            AppliedDiscountAmount = appliedDiscountAmount,
            OriginalBillAmount = originalBillAmount,
            FinalBillAmount = bill.TotalAmount.AmountRials,
            IsFreePayment = false,
            PaymentSkipped = false,
            PaymentStatus = "پرداخت عادی"
        };
    }
}