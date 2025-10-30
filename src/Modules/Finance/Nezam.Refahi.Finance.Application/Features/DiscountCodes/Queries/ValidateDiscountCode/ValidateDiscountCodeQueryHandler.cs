using MediatR;
using Microsoft.Extensions.Logging;
using Nezam.Refahi.Finance.Application.DiscountCodes.Queries;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Services;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.DiscountCodes.Queries.Validate;

public sealed class ValidateDiscountCodeHandler
    : IRequestHandler<ValidateDiscountCodeQuery, ApplicationResult<DiscountValidationDto>>
{
    private readonly IBillRepository _billRepository;
    private readonly IDiscountCodeRepository _discountCodeRepository;
    private readonly IDiscountCodeUsageRepository _discountCodeUsageRepository;
    private readonly DiscountCodeDomainService _discountDomain;
    private readonly ILogger<ValidateDiscountCodeHandler> _logger;

    public ValidateDiscountCodeHandler(
        IBillRepository billRepository,
        IDiscountCodeRepository discountCodeRepository,
        IDiscountCodeUsageRepository discountCodeUsageRepository,
        DiscountCodeDomainService discountDomain,
        ILogger<ValidateDiscountCodeHandler> logger)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _discountCodeRepository = discountCodeRepository ?? throw new ArgumentNullException(nameof(discountCodeRepository));
        _discountCodeUsageRepository = discountCodeUsageRepository ?? throw new ArgumentNullException(nameof(discountCodeUsageRepository));
        _discountDomain = discountDomain ?? throw new ArgumentNullException(nameof(discountDomain));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ApplicationResult<DiscountValidationDto>> Handle(
        ValidateDiscountCodeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            _logger.LogInformation("Validating discount code {Code} for bill {BillId} by user {UserId}",
                request.DiscountCode, request.BillId, request.ExternalUserId);

            // 1) Load bill
            var bill = await _billRepository.GetByIdAsync(request.BillId, cancellationToken);
            if (bill is null)
                return ApplicationResult<DiscountValidationDto>.Failure("Bill not found.");

            // 2) Ownership check
            if (bill.ExternalUserId != request.ExternalUserId)
                return ApplicationResult<DiscountValidationDto>.Failure("Unauthorized to access this bill.");

            // 3) Load discount code
            var code = await _discountCodeRepository.GetByCodeAsync(request.DiscountCode, cancellationToken);
            if (code is null)
                return ApplicationResult<DiscountValidationDto>.Failure("Discount code not found.");

            // 4) Usage check for user
            var hasUserUsed = await _discountCodeUsageRepository
                .HasUserUsedCodeAsync(code.Id, request.ExternalUserId, cancellationToken);

            // 5) Validate via domain service
            var validation = _discountDomain.ValidateDiscountCode(
                code, bill.TotalAmount, request.ExternalUserId, hasUserUsed);

            // 6) Compute amounts if valid
            decimal discountAmountRials = 0;
            decimal newTotalAmountRials = bill.TotalAmount.AmountRials;
            decimal? discountPercent = null;

            if (validation.IsValid)
            {
                var discountMoney = _discountDomain.CalculateDiscountAmount(code, bill.TotalAmount);
                discountAmountRials = discountMoney.AmountRials;
                newTotalAmountRials = Math.Max(0, bill.TotalAmount.AmountRials - discountAmountRials);

                if (code.Type == DiscountType.Percentage)
                    discountPercent = code.DiscountValue;
            }

            // 7) Build final DTO
            var dto = new DiscountValidationDto
            {
                IsValid = validation.IsValid,
                Errors = validation.ErrorMessage is null ? new List<string>() : new List<string> { validation.ErrorMessage },

                DiscountAmountRials = discountAmountRials,
                NewTotalAmountRials = newTotalAmountRials,
                DiscountPercentage = discountPercent,
                IsPercentageDiscount = code.Type == DiscountType.Percentage,
                IsFixedAmountDiscount = code.Type == DiscountType.FixedAmount,

                Bill = MapBillSnapshot(bill),
                DiscountCode = MapDiscountCodeSnapshot(code)
            };

            _logger.LogInformation("Discount validation finished. Valid={Valid}, NewTotal={NewTotal}",
                dto.IsValid, dto.NewTotalAmountRials);

            return ApplicationResult<DiscountValidationDto>.Success(dto, "Validation completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating discount code {Code} for bill {BillId}", request.DiscountCode, request.BillId);
            return ApplicationResult<DiscountValidationDto>.Failure(ex, "Validation failed.");
        }
    }

    // ---------------------- mapping helpers ----------------------

    private static BillDiscountSnapshotDto MapBillSnapshot(Bill bill)
    {
        var canApply = bill.Status is BillStatus.Draft or BillStatus.Issued;

        return new BillDiscountSnapshotDto
        {
            BillId = bill.Id,
            BillNumber = bill.BillNumber,
            Title = bill.Title,
            ReferenceId = bill.ReferenceId,
            BillType = bill.ReferenceType,

            Status = bill.Status.ToString(),
            StatusText = ToPersian(bill.Status),

            ExternalUserId = bill.ExternalUserId,
            UserFullName = bill.UserFullName,

            OriginalTotalAmountRials = bill.TotalAmount.AmountRials,
            PaidAmountRials = bill.PaidAmount.AmountRials,
            RemainingAmountRials = bill.RemainingAmount.AmountRials,

            AppliedDiscountCode = bill.DiscountCode,
            AppliedDiscountCodeId = bill.DiscountCodeId,
            HasAppliedDiscount = bill.DiscountCodeId.HasValue,

            IssueDate = bill.IssueDate,
            DueDate = bill.DueDate,
            FullyPaidDate = bill.FullyPaidDate,

            IsPaid = bill.Status == BillStatus.FullyPaid,
            IsPartiallyPaid = bill.Status == BillStatus.PartiallyPaid,
            IsOverdue = bill.IsOverdue() || bill.Status == BillStatus.Overdue,
            IsCancelled = bill.Status == BillStatus.Cancelled,
            CanApplyDiscount = canApply,

            Items = bill.Items.Select(i => new DiscountValidationItemDto
            {
                ItemId = i.Id,
                Title = i.Title,
                Description = i.Description,
                UnitPriceRials = i.UnitPrice.AmountRials,
                Quantity = i.Quantity,
                LineTotalRials = i.GetTotalAmount().AmountRials,
                ReferenceId = i.Title,
                Metadata = new Dictionary<string, string>()
            }).ToList()
        };
    }

    private static DiscountCodeSnapshotDto MapDiscountCodeSnapshot(DiscountCode code)
    {
        var now = DateTime.UtcNow;
        var isExpired = code.ValidTo < now;
        var isDepleted = code.UsageLimit.HasValue && code.UsedCount >= code.UsageLimit.Value;
        var isActive = code.Status == DiscountCodeStatus.Active && !isExpired && !isDepleted;

        var remaining = code.UsageLimit.HasValue
            ? Math.Max(0, code.UsageLimit.Value - code.UsedCount)
            : int.MaxValue;

        return new DiscountCodeSnapshotDto
        {
            DiscountCodeId = code.Id,
            Code = code.Code,
            Title = code.Title,
            Type = code.Type.ToString(),
            Status = code.Status.ToString(),
            Value = code.DiscountValue,
            ValidFrom = code.ValidFrom,
            ValidTo = code.ValidTo,
            UsageLimit = code.UsageLimit,
            CurrentUsages = code.UsedCount,
            RemainingUsages = remaining,
            IsSingleUse = code.IsSingleUse,
            Description = code.Description,
            MinimumBillAmountRials = code.MinimumBillAmount?.AmountRials,
            MaximumDiscountAmountRials = code.MaximumDiscountAmount?.AmountRials,
            IsExpired = isExpired,
            IsDepleted = isDepleted,
            IsActive = isActive
        };
    }

    private static string ToPersian(BillStatus status) =>
        status switch
        {
            BillStatus.Draft          => "پیش‌نویس",
            BillStatus.Issued         => "صادر شده",
            BillStatus.PartiallyPaid  => "پرداخت جزئی",
            BillStatus.FullyPaid      => "پرداخت کامل",
            BillStatus.Overdue        => "سررسید گذشته",
            BillStatus.Cancelled      => "لغو شده",
            BillStatus.Voided         => "باطل شده",
            BillStatus.WrittenOff     => "سوخت مطالبات",
            BillStatus.Credited       => "اعتبار اعمال شد",
            BillStatus.Disputed       => "دارای اختلاف",
            BillStatus.Refunded       => "مسترد شده",
            _                         => status.ToString()
        };
}
