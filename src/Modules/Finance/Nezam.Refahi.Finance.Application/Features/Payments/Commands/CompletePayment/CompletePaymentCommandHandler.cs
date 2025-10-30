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
using Nezam.Refahi.Contracts.Finance.v1.Messages;

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
    private readonly IBus _publishEndpoint;

    public CompletePaymentCommandHandler(
        IBillRepository billRepository,
        IPaymentRepository paymentRepository,
        IValidator<CompletePaymentCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        IBus publishEndpoint)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
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
				// Publish failure event
				await PublishPaymentFailedAsync(
					request.PaymentId,
					referenceId: null,
					referenceType: null,
					externalUserId: Guid.Empty,
					failureReason: "Validation failed",
					errorCode: "VALIDATION_FAILED",
					cancellationToken: cancellationToken);
				return ApplicationResult<CompletePaymentResponse>.Failure(errors, "Validation failed");
            }

            // Get payment
			var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
            {
				await PublishPaymentFailedAsync(
					request.PaymentId,
					referenceId: null,
					referenceType: null,
					externalUserId: Guid.Empty,
					failureReason: "Payment not found",
					errorCode: "PAYMENT_NOT_FOUND",
					cancellationToken: cancellationToken);
				await _unitOfWork.RollbackAsync(cancellationToken);
				return ApplicationResult<CompletePaymentResponse>.Failure("Payment not found");
            }

            // Get bill
			var bill = await _billRepository.GetByIdAsync(payment.BillId, cancellationToken);
            if (bill == null)
            {
				await PublishPaymentFailedAsync(
					payment.Id,
					referenceId: null,
					referenceType: null,
					externalUserId: Guid.Empty,
					failureReason: "Bill not found",
					errorCode: "BILL_NOT_FOUND",
					cancellationToken: cancellationToken);
				await _unitOfWork.RollbackAsync(cancellationToken);
				return ApplicationResult<CompletePaymentResponse>.Failure("Bill not found");
            }

            // Record payment in bill (this will mark payment as completed and update bill status)
            bill.RecordPayment(
                paymentId: request.PaymentId,
                gatewayTransactionId: request.GatewayTransactionId
            );

            // Get updated payment before saving
            var updatedPayment = bill.Payments.First(p => p.Id == request.PaymentId);

			// Publish Integration Events INSIDE transaction (Transactional Outbox Pattern)
            var paymentCompletedEvent = new PaymentCompletedIntegrationEvent
            {
                PaymentId = updatedPayment.Id,
                ReferenceId = bill.ReferenceId,
                ReferenceType = bill.ReferenceType,
                ExternalUserId = bill.ExternalUserId,
				AmountRials = (long)updatedPayment.Amount.AmountRials,
                GatewayTransactionId = request.GatewayTransactionId,
                GatewayReference = updatedPayment.GatewayReference,
                CompletedAt = updatedPayment.CompletedAt!.Value,
                Metadata = new Dictionary<string, string>
                {
                    ["BillId"] = bill.Id.ToString(),
                    ["BillNumber"] = bill.BillNumber,
                    ["PaymentMethod"] = updatedPayment.Method.ToString()
                }
            };
            
            // Publish with Idempotency Key for reliability
            var paymentIdempotencyKey = $"payment_completed_{updatedPayment.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}";
            await _publishEndpoint.Publish(paymentCompletedEvent, cancellationToken);

            // Check if bill is fully paid and publish BillFullyPaidCompletedIntegrationEvent
            if (bill.Status == Nezam.Refahi.Finance.Domain.Enums.BillStatus.FullyPaid)
            {
                var billFullyPaidCompletedEvent = new BillFullyPaidEventMessage()
                {
                    BillId = bill.Id,
                    BillNumber = bill.BillNumber,
                    ReferenceId = Guid.Parse(bill.ReferenceId),
                    ReferenceTrackingCode = bill.ReferenceTrackCode,
                    ReferenceType = bill.ReferenceType,
                    PaidAmountRials = (long)bill.TotalAmount.AmountRials,
                    PaidAt = updatedPayment.CompletedAt!.Value,
                    PaymentId = updatedPayment.Id,
                    GatewayTransactionId = request.GatewayTransactionId,
                    GatewayReference = updatedPayment.GatewayReference,
                    Gateway = updatedPayment.Gateway?.ToString() ?? "Unknown",
                    ExternalUserId = bill.ExternalUserId,
                    UserFullName = bill.UserFullName ?? string.Empty,
                    Metadata = new Dictionary<string, string>
                    {
                        ["PaymentMethod"] = updatedPayment.Method.ToString(),
                        ["CompletedAt"] = updatedPayment.CompletedAt!.Value.ToString("O"),
                        ["TotalAmount"] = bill.TotalAmount.AmountRials.ToString(),
                        ["PaymentCount"] = bill.Payments.Count.ToString(),
                        ["LastPaymentMethod"] = updatedPayment.Method.ToString(),
                        ["LastGateway"] = updatedPayment.Gateway?.ToString() ?? "Unknown"
                    }
                };
                
                // Publish with Idempotency Key for reliability
                var billIdempotencyKey = $"bill_fully_paid_{bill.Id}_{DateTime.UtcNow:yyyyMMddHHmmss}";
                await _publishEndpoint.Publish(billFullyPaidCompletedEvent, cancellationToken);
            }

            // Save changes (including Outbox messages) and commit transaction
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
			await _unitOfWork.CommitAsync(cancellationToken);

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
			await PublishPaymentFailedAsync(
				paymentId: Guid.Empty,
				referenceId: null,
				referenceType: null,
				externalUserId: Guid.Empty,
				failureReason: ex.Message,
				errorCode: "EXCEPTION",
				cancellationToken: cancellationToken);
			await _unitOfWork.RollbackAsync(cancellationToken);
			return ApplicationResult<CompletePaymentResponse>.Failure(ex, "Failed to complete payment");
        }
    }

	private async Task PublishPaymentFailedAsync(
		Guid paymentId,
		string? referenceId,
		string? referenceType,
		Guid externalUserId,
		string? failureReason,
		string? errorCode,
		CancellationToken cancellationToken)
	{
		var evt = new PaymentFailedIntegrationEvent
		{
			PaymentId = paymentId,
			ReferenceId = referenceId ?? string.Empty,
			ReferenceType = referenceType ?? string.Empty,
			ExternalUserId = externalUserId,
			FailureReason = failureReason ?? string.Empty,
			ErrorCode = errorCode,
			FailedAt = DateTime.UtcNow
		};

		await _publishEndpoint.Publish(evt, cancellationToken);
	}
}
