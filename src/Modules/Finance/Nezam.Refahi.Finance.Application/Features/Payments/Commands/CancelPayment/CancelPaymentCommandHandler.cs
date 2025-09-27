using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.CancelPayment;

/// <summary>
/// Handler for CancelPaymentCommand - Cancels a payment
/// </summary>
public class CancelPaymentCommandHandler : IRequestHandler<CancelPaymentCommand, ApplicationResult<CancelPaymentResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IValidator<CancelPaymentCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public CancelPaymentCommandHandler(
        IBillRepository billRepository,
        IPaymentRepository paymentRepository,
        IValidator<CancelPaymentCommand> validator,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<CancelPaymentResponse>> Handle(
        CancelPaymentCommand request,
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
                return ApplicationResult<CancelPaymentResponse>.Failure(errors, "Validation failed");
            }

            // Get payment
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
            if (payment == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CancelPaymentResponse>.Failure("Payment not found");
            }

            // Cancel payment directly
            payment.Cancel(request.Reason);

            // Save changes
            await _paymentRepository.UpdateAsync(payment, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Prepare response
            var response = new CancelPaymentResponse
            {
                PaymentId = payment.Id,
                BillId = payment.BillId,
                Status = payment.Status.ToString(),
                CancellationReason = payment.FailureReason // Cancel method sets reason in FailureReason
            };

            return ApplicationResult<CancelPaymentResponse>.Success(response, "Payment cancelled successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CancelPaymentResponse>.Failure(ex, "Failed to cancel payment");
        }
    }
}
