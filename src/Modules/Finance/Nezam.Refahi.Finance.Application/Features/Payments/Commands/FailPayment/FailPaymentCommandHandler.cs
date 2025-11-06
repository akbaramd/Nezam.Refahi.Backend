using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using MassTransit;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.FailPayment;

/// <summary>
/// Handler for FailPaymentCommand - Marks a payment as failed
/// </summary>
public class FailPaymentCommandHandler : IRequestHandler<FailPaymentCommand, ApplicationResult<FailPaymentResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IValidator<FailPaymentCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IBus _publishEndpoint;

    public FailPaymentCommandHandler(
        IBillRepository billRepository,
        IPaymentRepository paymentRepository,
        IValidator<FailPaymentCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        IBus publishEndpoint)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ApplicationResult<FailPaymentResponse>> Handle(
        FailPaymentCommand request,
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
                return ApplicationResult<FailPaymentResponse>.Failure(errors, "Validation failed");
            }

            // Get payment
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<FailPaymentResponse>.Failure("Payment not found");
            }

            // Get bill
            var bill = await _billRepository.GetByIdAsync(payment.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<FailPaymentResponse>.Failure("Bill not found");
            }

            // Mark payment as failed
            bill.MarkPaymentAsFailed(request.PaymentId, request.FailureReason);

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Get updated payment
            var updatedPayment = bill.Payments.First(p => p.Id == request.PaymentId);

            // Publish PaymentFailedIntegrationEvent
            var paymentFailedEvent = new PaymentFailedIntegrationEvent
            {
                PaymentId = updatedPayment.Id,
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                ReferenceId = bill.ReferenceId,
                ReferenceType = bill.ReferenceType,
                AmountRials = (long)updatedPayment.Amount.AmountRials,
                FailedAt = DateTime.UtcNow,
                FailureReason = updatedPayment.FailureReason ?? request.FailureReason ?? string.Empty,
                ErrorCode = "PAYMENT_FAILED",
                GatewayTransactionId = updatedPayment.GatewayTransactionId,
                Gateway = updatedPayment.Gateway?.ToString() ?? "Unknown",
                ExternalUserId = bill.ExternalUserId,
                UserFullName = bill.UserFullName ?? string.Empty,
                Metadata = new Dictionary<string, string>
                {
                    ["BillId"] = bill.Id.ToString(),
                    ["BillNumber"] = bill.BillNumber,
                    ["PaymentMethod"] = updatedPayment.Method.ToString(),
                    ["FailedAt"] = DateTime.UtcNow.ToString("O")
                }
            };
            await _publishEndpoint.Publish(paymentFailedEvent, cancellationToken);

            // Prepare response
            var response = new FailPaymentResponse
            {
                PaymentId = updatedPayment.Id,
                BillId = bill.Id,
                Status = updatedPayment.Status.ToString(),
                FailureReason = updatedPayment.FailureReason
            };

            return ApplicationResult<FailPaymentResponse>.Success(response, "Payment marked as failed");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<FailPaymentResponse>.Failure(ex, "Failed to mark payment as failed");
        }
    }
}
