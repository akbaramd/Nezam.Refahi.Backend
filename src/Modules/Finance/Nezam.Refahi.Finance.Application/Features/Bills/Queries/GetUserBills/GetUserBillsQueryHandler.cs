using FluentValidation;
using MediatR;
using Nezam.Refahi.Finance.Contracts.Queries.Bills;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Queries.GetUserBills;

/// <summary>
/// Handler for GetUserBillsQuery - Retrieves all bills for a specific user
/// </summary>
public class GetUserBillsQueryHandler : IRequestHandler<GetUserBillsQuery, ApplicationResult<UserBillsResponse>>
{
    private readonly IBillRepository _billRepository;
    private readonly IValidator<GetUserBillsQuery> _validator;

    public GetUserBillsQueryHandler(
        IBillRepository billRepository,
        IValidator<GetUserBillsQuery> validator)
    {
        _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<ApplicationResult<UserBillsResponse>> Handle(
        GetUserBillsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Validate request
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
            {
                var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return ApplicationResult<UserBillsResponse>.Failure(errors, "Validation failed");
            }

            // Get all bills for the user
            var allBills = await _billRepository.GetByUserNationalNumberAsync(request.UserNationalNumber, cancellationToken);
            var billsList = allBills.ToList();

            // Apply filters
            var filteredBills = ApplyFilters(billsList, request);

            // Apply sorting
            var sortedBills = ApplySorting(filteredBills, request.SortBy, request.SortDirection);

            // Calculate statistics
            var statistics = CalculateStatistics(billsList);

            // Apply pagination
            var totalCount = sortedBills.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var pagedBills = sortedBills
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Build response
            var response = new UserBillsResponse
            {
                UserNationalNumber = request.UserNationalNumber,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                Bills = pagedBills.Select(BuildUserBillSummary).ToList(),
                Statistics = statistics
            };

            return ApplicationResult<UserBillsResponse>.Success(response, "User bills retrieved successfully");
        }
        catch (Exception ex)
        {
            return ApplicationResult<UserBillsResponse>.Failure(ex, "Failed to retrieve user bills");
        }
    }

    private static IEnumerable<Domain.Entities.Bill> ApplyFilters(
        IEnumerable<Domain.Entities.Bill> bills,
        GetUserBillsQuery request)
    {
        var filtered = bills;

        // Filter by status
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<BillStatus>(request.Status, true, out var status))
        {
            filtered = filtered.Where(b => b.Status == status);
        }

        // Filter by bill type
        if (!string.IsNullOrEmpty(request.BillType))
        {
            filtered = filtered.Where(b => b.BillType.Equals(request.BillType, StringComparison.OrdinalIgnoreCase));
        }

        // Filter only overdue
        if (request.OnlyOverdue)
        {
            filtered = filtered.Where(b => b.IsOverdue());
        }

        // Filter only unpaid
        if (request.OnlyUnpaid)
        {
            filtered = filtered.Where(b => b.Status != BillStatus.FullyPaid && b.Status != BillStatus.Cancelled);
        }

        return filtered;
    }

    private static IEnumerable<Domain.Entities.Bill> ApplySorting(
        IEnumerable<Domain.Entities.Bill> bills,
        string sortBy,
        string sortDirection)
    {
        var isDescending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "issuedate" => isDescending
                ? bills.OrderByDescending(b => b.IssueDate)
                : bills.OrderBy(b => b.IssueDate),
            "duedate" => isDescending
                ? bills.OrderByDescending(b => b.DueDate)
                : bills.OrderBy(b => b.DueDate),
            "totalamount" => isDescending
                ? bills.OrderByDescending(b => b.TotalAmount.AmountRials)
                : bills.OrderBy(b => b.TotalAmount.AmountRials),
            "status" => isDescending
                ? bills.OrderByDescending(b => b.Status)
                : bills.OrderBy(b => b.Status),
            _ => isDescending
                ? bills.OrderByDescending(b => b.IssueDate)
                : bills.OrderBy(b => b.IssueDate)
        };
    }

    private static UserBillsStatisticsDto CalculateStatistics(IList<Domain.Entities.Bill> bills)
    {
        var totalBills = bills.Count;
        var paidBills = bills.Count(b => b.Status == BillStatus.FullyPaid);
        var unpaidBills = bills.Count(b => b.Status != BillStatus.FullyPaid && b.Status != BillStatus.Cancelled);
        var partiallyPaidBills = bills.Count(b => b.Status == BillStatus.PartiallyPaid);
        var overdueBills = bills.Count(b => b.IsOverdue());
        var cancelledBills = bills.Count(b => b.Status == BillStatus.Cancelled);

        var totalAmount = bills.Sum(b => b.TotalAmount.AmountRials);
        var paidAmount = bills.Sum(b => b.PaidAmount.AmountRials);
        var remainingAmount = bills.Sum(b => b.RemainingAmount.AmountRials);
        var overdueAmount = bills.Where(b => b.IsOverdue()).Sum(b => b.RemainingAmount.AmountRials);

        return new UserBillsStatisticsDto
        {
            TotalBills = totalBills,
            PaidBills = paidBills,
            UnpaidBills = unpaidBills,
            PartiallyPaidBills = partiallyPaidBills,
            OverdueBills = overdueBills,
            CancelledBills = cancelledBills,
            TotalAmountRials = totalAmount,
            PaidAmountRials = paidAmount,
            RemainingAmountRials = remainingAmount,
            OverdueAmountRials = overdueAmount
        };
    }

    private static UserBillSummaryDto BuildUserBillSummary(Domain.Entities.Bill bill)
    {
        var now = DateTime.UtcNow;
        var daysUntilDue = bill.DueDate.HasValue ? (int)(bill.DueDate.Value - now).TotalDays : 0;
        var daysOverdue = bill.IsOverdue() ? (int)(now - bill.DueDate!.Value).TotalDays : 0;

        var paymentCompletion = bill.TotalAmount.AmountRials > 0
            ? (decimal)bill.PaidAmount.AmountRials / bill.TotalAmount.AmountRials * 100
            : 0;

        return new UserBillSummaryDto
        {
            BillId = bill.Id,
            BillNumber = bill.BillNumber,
            Title = bill.Title,
            ReferenceId = bill.ReferenceId,
            BillType = bill.BillType,
            Status = bill.Status.ToString(),
            IsPaid = bill.Status == BillStatus.FullyPaid,
            IsPartiallyPaid = bill.Status == BillStatus.PartiallyPaid,
            IsOverdue = bill.Status == BillStatus.Overdue || bill.IsOverdue(),
            IsCancelled = bill.Status == BillStatus.Cancelled,
            TotalAmountRials = bill.TotalAmount.AmountRials,
            PaidAmountRials = bill.PaidAmount.AmountRials,
            RemainingAmountRials = bill.RemainingAmount.AmountRials,
            IssueDate = bill.IssueDate,
            DueDate = bill.DueDate,
            FullyPaidDate = bill.FullyPaidDate,
            PaymentCompletionPercentage = paymentCompletion,
            DaysUntilDue = Math.Max(0, daysUntilDue),
            DaysOverdue = Math.Max(0, daysOverdue),
            ItemsCount = bill.Items.Count,
            PaymentsCount = bill.Payments.Count
        };
    }
}