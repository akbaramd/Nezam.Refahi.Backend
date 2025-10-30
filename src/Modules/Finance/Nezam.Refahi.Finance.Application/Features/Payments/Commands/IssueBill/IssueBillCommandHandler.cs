using MediatR;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.IssueBill;

/// <summary>
/// Handler for IssueBillCommand - Issues draft bills
/// </summary>
public class IssueBillCommandHandler : IRequestHandler<IssueBillCommand, ApplicationResult<IssueBillResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public IssueBillCommandHandler(
        IBillRepository billRepository,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<IssueBillResponse>> Handle(
        IssueBillCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginAsync(cancellationToken);
        try
        {
            // Get bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<IssueBillResponse>.Failure("Bill not found");
            }

            // Security check
            if (bill.ExternalUserId != request.ExternalUserId)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<IssueBillResponse>.Failure("Access denied: You can only issue your own bills");
            }

            // Issue the bill
            bill.Issue();

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Prepare response
            var isFreeBill = bill.TotalAmount.AmountRials <= 0;
            var response = new IssueBillResponse
            {
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                TotalAmount = bill.TotalAmount.AmountRials,
                Status = bill.Status.ToString(),
                IssueDate = bill.IssueDate,
                IsFreeBill = isFreeBill
            };

            var message = $"صورتحساب '{bill.BillNumber}' با موفقیت صادر شد. " +
                         $"مبلغ کل: {bill.TotalAmount.AmountRials:N0} ریال.";

            if (isFreeBill)
            {
                message += " این صورتحساب رایگان است.";
            }

            return ApplicationResult<IssueBillResponse>.Success(response, message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<IssueBillResponse>.Failure(ex, "Failed to issue bill");
        }
    }
}
