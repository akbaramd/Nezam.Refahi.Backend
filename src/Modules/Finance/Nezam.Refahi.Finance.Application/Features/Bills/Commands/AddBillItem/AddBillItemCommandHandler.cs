using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.AddBillItem;

/// <summary>
/// Handler for AddBillItemCommand - Adds an item to an existing bill
/// </summary>
public class AddBillItemCommandHandler : IRequestHandler<AddBillItemCommand, ApplicationResult<AddBillItemResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IValidator<AddBillItemCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public AddBillItemCommandHandler(
        IBillRepository billRepository,
        IValidator<AddBillItemCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<AddBillItemResponse>> Handle(
        AddBillItemCommand request,
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
                return ApplicationResult<AddBillItemResponse>.Failure(errors, "Validation failed");
            }

            // Get bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<AddBillItemResponse>.Failure("Bill not found");
            }

            // Create money value object
            var unitPrice = Money.FromRials(request.UnitPriceRials);

            // Add item to bill
            bill.AddItem(
                title: request.Title,
                description: request.Description,
                unitPrice: unitPrice,
                quantity: request.Quantity,
                discountPercentage: request.DiscountPercentage
            );

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Get the newly added item (it will be the last one)
            var addedItem = bill.Items.LastOrDefault();

            // Prepare response
            var response = new AddBillItemResponse
            {
                BillId = bill.Id,
                ItemId = addedItem?.Id ?? Guid.Empty,
                TotalBillAmount = bill.TotalAmount.AmountRials
            };

            return ApplicationResult<AddBillItemResponse>.Success(response, "Bill item added successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<AddBillItemResponse>.Failure($"Failed to add bill item: {ex.Message}");
        }
    }
}