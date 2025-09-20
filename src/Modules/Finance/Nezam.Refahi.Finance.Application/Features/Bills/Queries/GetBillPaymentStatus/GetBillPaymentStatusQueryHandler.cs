using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Contracts.Queries.Bills;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Queries.GetBillPaymentStatus;

/// <summary>
/// Handler for GetBillPaymentStatusQuery - Retrieves bill payment status and related information
/// </summary>
public class GetBillPaymentStatusQueryHandler :
    IRequestHandler<GetBillPaymentStatusQuery, ApplicationResult<BillPaymentStatusResponse>>,
    IRequestHandler<GetBillPaymentStatusByNumberQuery, ApplicationResult<BillPaymentStatusResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IValidator<GetBillPaymentStatusQuery> _validator;
    private readonly IValidator<GetBillPaymentStatusByNumberQuery> _numberValidator;

    public GetBillPaymentStatusQueryHandler(
        IBillRepository billRepository,
        IValidator<GetBillPaymentStatusQuery> validator,
        IValidator<GetBillPaymentStatusByNumberQuery> numberValidator)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _numberValidator = numberValidator ?? throw new ArgumentNullException(nameof(numberValidator));
    }

    public async Task<ApplicationResult<BillPaymentStatusResponse>> Handle(
        GetBillPaymentStatusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<BillPaymentStatusResponse>.Failure(errors, "Validation failed");
            }

            // Get bill by ID with all related data for complete status information
            var bill = await _billRepository.GetWithAllDataAsync(request.BillId, cancellationToken);
            if (bill == null)
            {
                return ApplicationResult<BillPaymentStatusResponse>.Failure("Bill not found");
            }

            // Build response
            var response = BuildBillPaymentStatusResponse(bill, request.IncludePaymentHistory, request.IncludeRefundHistory, request.IncludeBillItems);

            return ApplicationResult<BillPaymentStatusResponse>.Success(response, "Bill payment status retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApplicationResult<BillPaymentStatusResponse>.Failure($"Failed to retrieve bill payment status: {ex.Message}");
        }
    }

    public async Task<ApplicationResult<BillPaymentStatusResponse>> Handle(
        GetBillPaymentStatusByNumberQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            var validation = await _numberValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<BillPaymentStatusResponse>.Failure(errors, "Validation failed");
            }

            // Get bill by number - first get basic bill, then get full data if found
            var basicBill = await _billRepository.GetByBillNumberAsync(request.BillNumber, cancellationToken);
            if (basicBill == null)
            {
                return ApplicationResult<BillPaymentStatusResponse>.Failure("Bill not found");
            }

            // Get bill with all related data for complete status information
            var bill = await _billRepository.GetWithAllDataAsync(basicBill.Id, cancellationToken);
            if (bill == null)
            {
                return ApplicationResult<BillPaymentStatusResponse>.Failure("Bill not found");
            }

            // Build response
            var response = BuildBillPaymentStatusResponse(bill, request.IncludePaymentHistory, request.IncludeRefundHistory, request.IncludeBillItems);

            return ApplicationResult<BillPaymentStatusResponse>.Success(response, "Bill payment status retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApplicationResult<BillPaymentStatusResponse>.Failure($"Failed to retrieve bill payment status: {ex.Message}");
        }
    }

    private static BillPaymentStatusResponse BuildBillPaymentStatusResponse(
        Domain.Entities.Bill bill,
        bool includePaymentHistory,
        bool includeRefundHistory,
        bool includeBillItems)
    {
        var now = DateTime.UtcNow;
        var daysUntilDue = bill.DueDate.HasValue ? (int)(bill.DueDate.Value - now).TotalDays : 0;
        var daysOverdue = bill.IsOverdue() ? (int)(now - bill.DueDate!.Value).TotalDays : 0;

        var paymentCompletion = bill.TotalAmount.AmountRials > 0
            ? (decimal)bill.PaidAmount.AmountRials / bill.TotalAmount.AmountRials * 100
            : 0;

        var response = new BillPaymentStatusResponse
        {
            BillId = bill.Id,
            BillNumber = bill.BillNumber,
            Title = bill.Title,
            ReferenceId = bill.ReferenceId,
            BillType = bill.BillType,
            UserNationalNumber = bill.UserNationalNumber,
            UserFullName = bill.UserFullName,
            Status = bill.Status.ToString(),
            IsPaid = bill.Status == BillStatus.FullyPaid,
            IsPartiallyPaid = bill.Status == BillStatus.PartiallyPaid,
            IsOverdue = bill.Status == BillStatus.Overdue || bill.IsOverdue(),
            IsCancelled = bill.Status == BillStatus.Cancelled,
            TotalAmountRials = bill.TotalAmount.AmountRials,
            PaidAmountRials = bill.PaidAmount.AmountRials,
            RemainingAmountRials = bill.RemainingAmount.AmountRials,
            Description = bill.Description,
            IssueDate = bill.IssueDate,
            DueDate = bill.DueDate,
            FullyPaidDate = bill.FullyPaidDate,
            Metadata = bill.Metadata,
            PaymentCompletionPercentage = paymentCompletion,
            DaysUntilDue = Math.Max(0, daysUntilDue),
            DaysOverdue = Math.Max(0, daysOverdue)
        };

        // Add payment history if requested
        if (includePaymentHistory)
        {
            response = response with
            {
                PaymentHistory = bill.Payments.Select(p => new BillPaymentHistoryDto
                {
                    PaymentId = p.Id,
                    AmountRials = p.Amount.AmountRials,
                    Method = p.Method.ToString(),
                    Status = p.Status.ToString(),
                    Gateway = p.Gateway?.ToString(),
                    GatewayTransactionId = p.GatewayTransactionId,
                    GatewayReference = p.GatewayReference,
                    CreatedAt = p.CreatedAt,
                    CompletedAt = p.CompletedAt,
                    FailureReason = p.FailureReason
                }).ToList()
            };
        }

        // Add refund history if requested
        if (includeRefundHistory)
        {
            response = response with
            {
                RefundHistory = bill.Refunds.Select(r => new BillRefundHistoryDto
                {
                    RefundId = r.Id,
                    AmountRials = r.Amount.AmountRials,
                    Reason = r.Reason,
                    Status = r.Status.ToString(),
                    RequestedByNationalNumber = r.RequestedByNationalNumber,
                    RequestedAt = r.RequestedAt,
                    CompletedAt = r.CompletedAt,
                    GatewayRefundId = r.GatewayRefundId,
                    GatewayReference = r.GatewayReference
                }).ToList()
            };
        }

        // Add bill items if requested
        if (includeBillItems)
        {
            response = response with
            {
                BillItems = bill.Items.Select(i => new BillItemSummaryDto
                {
                    ItemId = i.Id,
                    Title = i.Title,
                    Description = i.Description,
                    UnitPriceRials = i.UnitPrice.AmountRials,
                    Quantity = i.Quantity,
                    DiscountPercentage = i.DiscountPercentage,
                    LineTotalRials = i.LineTotal.AmountRials
                }).ToList()
            };
        }

        return response;
    }
}