using MediatR;
using Nezam.Refahi.Finance.Application.Commands.Payments;
using Nezam.Refahi.Finance.Application.Services;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Services;
using Nezam.Refahi.Shared.Application.Common.Interfaces;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Payments.Commands.ApplyDiscountCode;

/// <summary>
/// Handler for ApplyDiscountCodeCommand - Applies discount codes to bills
/// </summary>
public class ApplyDiscountCodeCommandHandler : IRequestHandler<ApplyDiscountCodeCommand, ApplicationResult<ApplyDiscountCodeResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IDiscountCodeRepository _discountCodeRepository;
    private readonly IDiscountCodeUsageRepository _discountCodeUsageRepository;
    private readonly DiscountCodeDomainService _discountCodeDomainService;
    private readonly IFinanceUnitOfWork _unitOfWork;

    public ApplyDiscountCodeCommandHandler(
        IBillRepository billRepository,
        IDiscountCodeRepository discountCodeRepository,
        IDiscountCodeUsageRepository discountCodeUsageRepository,
        DiscountCodeDomainService discountCodeDomainService,
        IFinanceUnitOfWork unitOfWork)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _discountCodeRepository = discountCodeRepository ?? throw new ArgumentNullException(nameof(discountCodeRepository));
        _discountCodeUsageRepository = discountCodeUsageRepository ?? throw new ArgumentNullException(nameof(discountCodeUsageRepository));
        _discountCodeDomainService = discountCodeDomainService ?? throw new ArgumentNullException(nameof(discountCodeDomainService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<ApplicationResult<ApplyDiscountCodeResponse>> Handle(
        ApplyDiscountCodeCommand request,
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
                return ApplicationResult<ApplyDiscountCodeResponse>.Failure("Bill not found");
            }

            // Security check
            if (bill.ExternalUserId != request.ExternalUserId)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ApplyDiscountCodeResponse>.Failure("Access denied: You can only apply discount codes to your own bills");
            }

            // Get discount code
            var discountCode = await _discountCodeRepository.GetByCodeAsync(request.DiscountCode, cancellationToken);
            if (discountCode == null)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ApplyDiscountCodeResponse>.Failure("کد تخفیف یافت نشد");
            }

            // Check if user has already used this code
            var hasUserUsedCode = await _discountCodeUsageRepository.HasUserUsedCodeAsync(
                discountCode.Id, request.ExternalUserId, cancellationToken);

            // Validate discount code
            var validationResult = _discountCodeDomainService.ValidateDiscountCode(
                discountCode, bill.TotalAmount, request.ExternalUserId, hasUserUsedCode);

            if (!validationResult.IsValid)
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                return ApplicationResult<ApplyDiscountCodeResponse>.Failure(
                    validationResult.ErrorMessage ?? "کد تخفیف نامعتبر است");
            }

            // Store original amounts
            var originalBillAmount = bill.TotalAmount.AmountRials;

            // Calculate discount amount
            var discountAmount = _discountCodeDomainService.CalculateDiscountAmount(
                discountCode, bill.TotalAmount);

            // Apply discount to bill
            bill.ApplyDiscountCode(discountCode.Id, discountCode.Code, discountAmount);

            // Record usage using the domain method
            var (finalDiscountAmount, newUsage) = discountCode.ApplyDiscount(
                bill.TotalAmount, 
                bill.Id, 
                request.ExternalUserId, 
                bill.UserFullName);

            // Add the new usage to the repository context
            await _discountCodeUsageRepository.AddAsync(newUsage);

            // Save changes
            await _billRepository.UpdateAsync(bill, cancellationToken:cancellationToken);
            await _unitOfWork.SaveAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            // Prepare response
            var finalBillAmount = bill.TotalAmount.AmountRials;
            var isFreeBill = finalBillAmount <= 0;

            var response = new ApplyDiscountCodeResponse
            {
                BillId = bill.Id,
                BillNumber = bill.BillNumber,
                AppliedDiscountCode = discountCode.Code,
                AppliedDiscountAmount = discountAmount.AmountRials,
                OriginalBillAmount = originalBillAmount,
                FinalBillAmount = finalBillAmount,
                IsFreeBill = isFreeBill,
                Status = isFreeBill ? "صورتحساب رایگان" : "تخفیف اعمال شد"
            };

            var message = $"کد تخفیف '{discountCode.Code}' با موفقیت اعمال شد. " +
                         $"مبلغ تخفیف: {discountAmount.AmountRials:N0} ریال. " +
                         $"مبلغ نهایی: {finalBillAmount:N0} ریال.";

            if (isFreeBill)
            {
                message += " صورتحساب اکنون رایگان است.";
            }

            return ApplicationResult<ApplyDiscountCodeResponse>.Success(response, message);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return ApplicationResult<ApplyDiscountCodeResponse>.Failure(ex, "Failed to apply discount code");
        }
    }
}
