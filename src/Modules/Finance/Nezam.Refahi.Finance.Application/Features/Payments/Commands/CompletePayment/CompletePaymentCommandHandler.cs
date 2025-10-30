using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Infrastructure.Outbox;

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
    private readonly IOutboxPublisher _outboxPublisher;

    public CompletePaymentCommandHandler(
        IBillRepository billRepository,
        IPaymentRepository paymentRepository,
        IValidator<CompletePaymentCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        IOutboxPublisher outboxPublisher)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _outboxPublisher = outboxPublisher ?? throw new ArgumentNullException(nameof(outboxPublisher));
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
            await _outboxPublisher.PublishAsync(
                paymentCompletedEvent, 
                aggregateId: bill.Id,
                correlationId: request.GatewayTransactionId,
                idempotencyKey: paymentIdempotencyKey,
                cancellationToken);

            // Check if bill is fully paid and publish BillFullyPaidCompletedIntegrationEvent
            if (bill.Status == Nezam.Refahi.Finance.Domain.Enums.BillStatus.FullyPaid)
            {
                var billFullyPaidCompletedEvent = new BillFullyPaidCompletedIntegrationEvent
                {
                    BillId = bill.Id,
                    BillNumber = bill.BillNumber,
                    ReferenceId = bill.ReferenceId,
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
                await _outboxPublisher.PublishAsync(
                    billFullyPaidCompletedEvent, 
                    aggregateId: bill.Id,
                    correlationId: request.GatewayTransactionId,
                    idempotencyKey: billIdempotencyKey,
                    cancellationToken);
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
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CompletePaymentResponse>.Failure(ex, "Failed to complete payment");
        }
    }
}
