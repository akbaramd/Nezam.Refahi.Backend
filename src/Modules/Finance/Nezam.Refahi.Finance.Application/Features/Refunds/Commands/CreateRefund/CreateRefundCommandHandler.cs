using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Refunds;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Refunds.Commands.CreateRefund;

/// <summary>
/// Handler for CreateRefundCommand - Creates a refund request for a bill
/// </summary>
public class CreateRefundCommandHandler : IRequestHandler<CreateRefundCommand, ApplicationResult<CreateRefundResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IRefundRepository _refundRepository;
    private readonly IValidator<CreateRefundCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public CreateRefundCommandHandler(
        IBillRepository billRepository,
        IRefundRepository refundRepository,
        IValidator<CreateRefundCommand> validator,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _refundRepository = refundRepository ?? throw new ArgumentNullException(nameof(refundRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<CreateRefundResponse>> Handle(
        CreateRefundCommand request,
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
                return ApplicationResult<CreateRefundResponse>.Failure(errors, "Validation failed");
            }

            // Get bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CreateRefundResponse>.Failure("Bill not found");
            }

            // Create money value object
            var refundAmount = Money.FromRials(request.RefundAmountRials);

            // Create refund
            var refund = bill.CreateRefund(
                refundAmount: refundAmount,
                reason: request.Reason,
                requestedByExternalUserId: request.RequestedByExternalUserId
            );

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Prepare response
            var response = new CreateRefundResponse
            {
                RefundId = refund.Id,
                BillId = bill.Id,
                RefundAmount = refund.Amount.AmountRials,
                Status = refund.Status.ToString(),
                RequestedAt = refund.RequestedAt
            };

            return ApplicationResult<CreateRefundResponse>.Success(response, "Refund request created successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CreateRefundResponse>.Failure(ex, "Failed to create refund");
        }
    }
}
