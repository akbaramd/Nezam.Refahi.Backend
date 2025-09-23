using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.RemoveBillItem;

/// <summary>
/// Handler for RemoveBillItemCommand - Removes an item from an existing bill
/// </summary>
public class RemoveBillItemCommandHandler : IRequestHandler<RemoveBillItemCommand, ApplicationResult<RemoveBillItemResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IValidator<RemoveBillItemCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public RemoveBillItemCommandHandler(
        IBillRepository billRepository,
        IValidator<RemoveBillItemCommand> validator,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<RemoveBillItemResponse>> Handle(
        RemoveBillItemCommand request,
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
                return ApplicationResult<RemoveBillItemResponse>.Failure(errors, "Validation failed");
            }

            // Get bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<RemoveBillItemResponse>.Failure("Bill not found");
            }

            // Check if item exists
            var item = bill.Items.FirstOrDefault(i => i.Id == request.ItemId);
            if (item == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<RemoveBillItemResponse>.Failure("Bill item not found");
            }

            // Remove item from bill
            bill.RemoveItem(request.ItemId);

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Prepare response
            var response = new RemoveBillItemResponse
            {
                BillId = bill.Id,
                TotalBillAmount = bill.TotalAmount.AmountRials
            };

            return ApplicationResult<RemoveBillItemResponse>.Success(response, "Bill item removed successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<RemoveBillItemResponse>.Failure(ex, "Failed to remove bill item");
        }
    }
}