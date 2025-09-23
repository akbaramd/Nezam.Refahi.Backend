using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Contracts.Commands.Bills;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Commands.IssueBill;

/// <summary>
/// Handler for IssueBillCommand - Issues a bill (finalizes it and makes it ready for payment)
/// </summary>
public class IssueBillCommandHandler : IRequestHandler<IssueBillCommand, ApplicationResult<IssueBillResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IValidator<IssueBillCommand> _validator;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public IssueBillCommandHandler(
        IBillRepository billRepository,
        IValidator<IssueBillCommand> validator,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<IssueBillResponse>> Handle(
        IssueBillCommand request,
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
                return ApplicationResult<IssueBillResponse>.Failure(errors, "Validation failed: "+string.Join(',', errors));
            }

            // Get bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<IssueBillResponse>.Failure("Bill not found");
            }

            // Issue the bill
            bill.Issue();

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Prepare response
            var response = new IssueBillResponse
            {
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                Status = bill.Status.ToString(),
                IssueDate = bill.IssueDate,
                TotalAmount = bill.TotalAmount.AmountRials
            };

            return ApplicationResult<IssueBillResponse>.Success(response, "Bill issued successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<IssueBillResponse>.Failure(ex, "Failed to issue bill");
        }
    }
}