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
    IRequestHandler<GetBillPaymentStatusByNumberQuery, ApplicationResult<BillPaymentStatusResponse>>,
    IRequestHandler<GetBillPaymentStatusByTrackingCodeQuery, ApplicationResult<BillPaymentStatusResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IValidator<GetBillPaymentStatusQuery> _validator;
    private readonly IValidator<GetBillPaymentStatusByNumberQuery> _numberValidator;
    private readonly IValidator<GetBillPaymentStatusByTrackingCodeQuery> _trackingCodeValidator;

    public GetBillPaymentStatusQueryHandler(
        IBillRepository billRepository,
        IValidator<GetBillPaymentStatusQuery> validator,
        IValidator<GetBillPaymentStatusByNumberQuery> numberValidator,
        IValidator<GetBillPaymentStatusByTrackingCodeQuery> trackingCodeValidator)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _numberValidator = numberValidator ?? throw new ArgumentNullException(nameof(numberValidator));
        _trackingCodeValidator = trackingCodeValidator ?? throw new ArgumentNullException(nameof(trackingCodeValidator));
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
            return ApplicationResult<BillPaymentStatusResponse>.Failure(ex, "Failed to retrieve bill payment status");
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
            return ApplicationResult<BillPaymentStatusResponse>.Failure(ex, "Failed to retrieve bill payment status");
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
            UserExternalUserId = bill.ExternalUserId,
            UserFullName = bill.UserFullName,
            Status = bill.Status.ToString(),
            StatusText = GetBillStatusText(bill.Status),
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
                    StatusText = GetPaymentStatusText(p.Status),
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
                    StatusText = GetRefundStatusText(r.Status),
                    RequestedByExternalUserId = r.RequestedByExternalUserId,
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

    public async Task<ApplicationResult<BillPaymentStatusResponse>> Handle(
        GetBillPaymentStatusByTrackingCodeQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            var validation = await _trackingCodeValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<BillPaymentStatusResponse>.Failure(errors, "Validation failed");
            }

            // Get bill by tracking code (reference ID) and bill type
            var bill = await _billRepository.GetByReferenceAsync(request.TrackingCode, request.BillType, cancellationToken);
            if (bill == null)
            {
                return ApplicationResult<BillPaymentStatusResponse>.Failure("Bill not found for the given tracking code");
            }

            // Get bill with all related data for complete status information
            var billWithData = await _billRepository.GetWithAllDataAsync(bill.Id, cancellationToken);
            if (billWithData == null)
            {
                return ApplicationResult<BillPaymentStatusResponse>.Failure("Bill data not found");
            }

            // Build response with tracking code information
            var response = BuildBillPaymentStatusResponse(billWithData, request.IncludePaymentHistory, request.IncludeRefundHistory, request.IncludeBillItems);
            
            // Add tracking code to response
            response = response with
            {
                TrackingCode = request.TrackingCode
            };

            return ApplicationResult<BillPaymentStatusResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return ApplicationResult<BillPaymentStatusResponse>.Failure(ex, "Error retrieving bill payment status by tracking code");
        }
    }

    /// <summary>
    /// Convert bill status to Persian text
    /// </summary>
    private static string GetBillStatusText(BillStatus status)
    {
        return status switch
        {
            BillStatus.Draft => "پیش‌نویس",
            BillStatus.Issued => "صادر شده",
            BillStatus.PartiallyPaid => "پرداخت جزئی",
            BillStatus.FullyPaid => "پرداخت کامل",
            BillStatus.Overdue => "منقضی شده",
            BillStatus.Cancelled => "لغو شده",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// Convert payment status to Persian text
    /// </summary>
    private static string GetPaymentStatusText(PaymentStatus status)
    {
        return status switch
        {
            PaymentStatus.Pending => "در انتظار",
            PaymentStatus.Processing => "در حال پردازش",
            PaymentStatus.Completed => "تکمیل شده",
            PaymentStatus.Failed => "ناموفق",
            PaymentStatus.Cancelled => "لغو شده",
            PaymentStatus.Refunded => "برگشت داده شده",
            _ => status.ToString()
        };
    }

    /// <summary>
    /// Convert refund status to Persian text
    /// </summary>
    private static string GetRefundStatusText(RefundStatus status)
    {
        return status switch
        {
            RefundStatus.Pending => "در انتظار",
            RefundStatus.Processing => "در حال پردازش",
            RefundStatus.Completed => "تکمیل شده",
            RefundStatus.Rejected => "رد شده",
            RefundStatus.Failed => "ناموفق",
            _ => status.ToString()
        };
    }
}