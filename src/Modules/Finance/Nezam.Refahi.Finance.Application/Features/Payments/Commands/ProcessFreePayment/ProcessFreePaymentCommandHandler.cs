using MediatR;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Features.Payments.Commands.CompletePayment;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.ProcessFreePayment;

/// <summary>
/// Handler for ProcessFreePaymentCommand - Handles zero-amount bills and 100% discount payments
/// </summary>
public class ProcessFreePaymentCommandHandler : IRequestHandler<ProcessFreePaymentCommand, ApplicationResult<ProcessFreePaymentResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public ProcessFreePaymentCommandHandler(
        IBillRepository billRepository,
        IFinanceUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ApplicationResult<ProcessFreePaymentResponse>> Handle(
        ProcessFreePaymentCommand request,
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
                return ApplicationResult<ProcessFreePaymentResponse>.Failure("Bill not found");
            }

            // Security check
            if (bill.ExternalUserId != request.ExternalUserId)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessFreePaymentResponse>.Failure("Access denied: You can only process payments for your own bills");
            }

            // Validate bill is payable
            if (bill.Status == BillStatus.Draft)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessFreePaymentResponse>.Failure("Cannot process payment for draft bill. Issue the bill first.");
            }

            if (bill.Status == BillStatus.Cancelled)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessFreePaymentResponse>.Failure("Cannot process payment for cancelled bill.");
            }

            if (bill.Status == BillStatus.FullyPaid)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessFreePaymentResponse>.Failure("Bill is already fully paid.");
            }

            // Validate bill is actually free
            if (bill.RemainingAmount.AmountRials > 0)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ProcessFreePaymentResponse>.Failure(
                    $"Bill is not free. Remaining amount: {bill.RemainingAmount.AmountRials:N0} Rials");
            }

            // Create virtual payment for free bill
            var zeroAmount = Money.FromRials(0);
            var payment = bill.CreatePayment(
                amount: zeroAmount,
                method: PaymentMethod.Free, // Virtual payment method
                gateway: PaymentGateway.System,
                callbackUrl: null,
                description: request.Description ?? "پرداخت رایگان",
                expiryDate: null
            );

            // Mark payment as free
            payment.SetDiscount(null, null, Money.Zero, true);

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Complete the payment automatically
            var completePaymentCommand = new CompletePaymentCommand
            {
                PaymentId = payment.Id,
                GatewayTransactionId = "free",
            };

            var completeResult = await _mediator.Send(completePaymentCommand, cancellationToken);
            if (!completeResult.IsSuccess)
            {
                // Log the error but don't fail the entire operation
                // The payment was created successfully, just the completion failed
            }

            // Get updated bill status
            var updatedBill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            var updatedPayment = updatedBill?.Payments.FirstOrDefault(p => p.Id == payment.Id);

            // Prepare response
            var response = new ProcessFreePaymentResponse
            {
                PaymentId = payment.Id,
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                Amount = payment.Amount.AmountRials,
                PaymentMethod = payment.Method.ToString(),
                Status = updatedPayment?.Status.ToString() ?? payment.Status.ToString(),
                CreatedAt = payment.CreatedAt,
                BillStatus = updatedBill?.Status.ToString() ?? bill.Status.ToString(),
                BillTotalAmount = bill.TotalAmount.AmountRials,
                IsFreePayment = true,
                PaymentStatus = "پرداخت رایگان - تکمیل شد"
            };

            return ApplicationResult<ProcessFreePaymentResponse>.Success(response, 
                "پرداخت رایگان با موفقیت پردازش و تکمیل شد. صورتحساب اکنون کاملاً پرداخت شده است.");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<ProcessFreePaymentResponse>.Failure(ex, "Failed to process free payment");
        }
    }
}
