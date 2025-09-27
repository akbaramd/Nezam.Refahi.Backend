using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Application.Commands.Bills;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.CancelBill;

/// <summary>
/// Handler for CancelBillCommand - Cancels a bill
/// </summary>
public class CancelBillCommandHandler : IRequestHandler<CancelBillCommand, ApplicationResult<CancelBillResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IValidator<CancelBillCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public CancelBillCommandHandler(
        IBillRepository billRepository,
        IValidator<CancelBillCommand> validator,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<CancelBillResponse>> Handle(
        CancelBillCommand request,
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
                return ApplicationResult<CancelBillResponse>.Failure(errors, "Validation failed");
            }

            // Get bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<CancelBillResponse>.Failure("Bill not found");
            }

            // Cancel the bill
            bill.Cancel(request.Reason);

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Prepare response
            var response = new CancelBillResponse
            {
                BillId = bill.Id,
                Status = bill.Status.ToString(),
                CancellationReason = bill.Metadata.TryGetValue("CancellationReason", out var reason) ? reason : null
            };

            return ApplicationResult<CancelBillResponse>.Success(response, "Bill cancelled successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<CancelBillResponse>.Failure(ex, "Failed to cancel bill");
        }
    }
}
