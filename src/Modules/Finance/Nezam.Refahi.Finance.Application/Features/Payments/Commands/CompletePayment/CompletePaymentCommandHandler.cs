using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Contracts.Commands.Payments;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.CompletePayment;

/// <summary>
/// Handler for CompletePaymentCommand - Completes a payment (marks as successful)
/// </summary>
public class CompletePaymentCommandHandler : IRequestHandler<CompletePaymentCommand, ApplicationResult<CompletePaymentResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IValidator<CompletePaymentCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public CompletePaymentCommandHandler(
        IBillRepository billRepository,
        IPaymentRepository paymentRepository,
        IValidator<CompletePaymentCommand> validator,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<CompletePaymentResponse>> Handle(
        CompletePaymentCommand request,
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
                return ApplicationResult<CompletePaymentResponse>.Failure(errors, "Validation failed");
            }

            // Get payment
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CompletePaymentResponse>.Failure("Payment not found");
            }

            // Get bill
            var bill = await _billRepository.GetByIdAsync(payment.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CompletePaymentResponse>.Failure("Bill not found");
            }

            // Record payment in bill (this will mark payment as completed and update bill status)
            bill.RecordPayment(
                paymentId: request.PaymentId,
                gatewayTransactionId: request.GatewayTransactionId,
                gatewayReference: request.GatewayReference
            );

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Get updated payment
            var updatedPayment = bill.Payments.First(p => p.Id == request.PaymentId);

            // Prepare response
            var response = new CompletePaymentResponse
            {
                PaymentId = updatedPayment.Id,
                BillId = bill.Id,
                Status = updatedPayment.Status.ToString(),
                CompletedAt = updatedPayment.CompletedAt!.Value,
                BillStatus = bill.Status.ToString(),
                BillRemainingAmount = bill.RemainingAmount.AmountRials
            };

            return ApplicationResult<CompletePaymentResponse>.Success(response, "Payment completed successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CompletePaymentResponse>.Failure(ex, "Failed to complete payment");
        }
    }
}