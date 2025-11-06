using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Refunds;
using Nezam.Refahi.Finance.Contracts.IntegrationEvents;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using MassTransit;

namespace Nezam.Refahi.Finance.Application.Features.Refunds.Commands.CompleteRefund;

/// <summary>
/// Handler for CompleteRefundCommand - Completes a refund (marks as successful)
/// </summary>
public class CompleteRefundCommandHandler : IRequestHandler<CompleteRefundCommand, ApplicationResult<CompleteRefundResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IRefundRepository _refundRepository;
    private readonly IValidator<CompleteRefundCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IBus _publishEndpoint;

    public CompleteRefundCommandHandler(
        IBillRepository billRepository,
        IRefundRepository refundRepository,
        IValidator<CompleteRefundCommand> validator,
        IFinanceUnitOfWork unitOfWork,
        IBus publishEndpoint)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _refundRepository = refundRepository ?? throw new ArgumentNullException(nameof(refundRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
    }

    public async Task<ApplicationResult<CompleteRefundResponse>> Handle(
        CompleteRefundCommand request,
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
                return ApplicationResult<CompleteRefundResponse>.Failure(errors, "Validation failed");
            }

            // Get refund
            var refund = await _refundRepository.GetByIdAsync(request.RefundId, cancellationToken);
            if (refund == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CompleteRefundResponse>.Failure("Refund not found");
            }

            // Get bill
            var bill = await _billRepository.GetByIdAsync(refund.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CompleteRefundResponse>.Failure("Bill not found");
            }

            // Complete refund in bill (this will mark refund as completed and update bill status)
            bill.CompleteRefund(
                refundId: request.RefundId,
                gatewayRefundId: request.GatewayRefundId,
                gatewayReference: request.GatewayReference
            );

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Get updated refund
            var updatedRefund = bill.Refunds.First(r => r.Id == request.RefundId);

            // Publish RefundCompletedIntegrationEvent
            var refundCompletedEvent = new RefundCompletedIntegrationEvent
            {
                RefundId = updatedRefund.Id,
                PaymentId = bill.Payments.FirstOrDefault()?.Id ?? Guid.Empty,
                ReferenceId = bill.ReferenceId,
                ReferenceType = bill.ReferenceType,
                RefundAmountRials = (long)updatedRefund.Amount.AmountRials,
                RequestedByNationalNumber = bill.ExternalUserId.ToString(),
                GatewayRefundId = request.GatewayRefundId,
                CompletedAt = updatedRefund.CompletedAt!.Value
            };
            await _publishEndpoint.Publish(refundCompletedEvent, cancellationToken);

            // Prepare response
            var response = new CompleteRefundResponse
            {
                RefundId = updatedRefund.Id,
                BillId = bill.Id,
                Status = updatedRefund.Status.ToString(),
                CompletedAt = updatedRefund.CompletedAt!.Value,
                BillStatus = bill.Status.ToString(),
                BillRemainingAmount = bill.RemainingAmount.AmountRials
            };

            return ApplicationResult<CompleteRefundResponse>.Success(response, "Refund completed successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CompleteRefundResponse>.Failure(ex, "Failed to complete refund");
        }
    }
}
