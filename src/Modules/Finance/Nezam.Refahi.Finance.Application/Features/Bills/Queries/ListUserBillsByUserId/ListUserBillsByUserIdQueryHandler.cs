using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using MCA.SharedKernel.Application.Contracts;
using MCA.SharedKernel.Domain.Models;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Application.Mappers;
using Nezam.Refahi.Finance.Application.Queries.Bills;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Queries.GetUserBills
{
    /// <summary>
    /// Handler returns ONLY BillDto (no custom response wrapper), paginated.
    /// </summary>
    public sealed class GetUserBillsQueryHandler
        : IRequestHandler<ListUserBillsByUserIdQuery, ApplicationResult<PaginatedResult<BillDto>>>
    {
        private readonly IBillRepository _billRepository;
        private readonly IValidator<ListUserBillsByUserIdQuery> _validator;
        private readonly IMapper<Bill, BillDto> _billMapper;

        public GetUserBillsQueryHandler(
            IBillRepository billRepository,
            IValidator<ListUserBillsByUserIdQuery> validator,
            IMapper<Bill, BillDto> billMapper)
        {
            _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _billMapper = billMapper ?? throw new ArgumentNullException(nameof(billMapper));
        }

        public async Task<ApplicationResult<PaginatedResult<BillDto>>> Handle(
            ListUserBillsByUserIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate
                var validation = await _validator.ValidateAsync(request, cancellationToken);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    return ApplicationResult<PaginatedResult<BillDto>>.Failure(errors, "Validation failed.");
                }

                // Load all user's bills (repository should be optimized/indexed by ExternalUserId)
                var allBills = await _billRepository.GetByExternalUserIdAsync(request.ExternalUserId, cancellationToken);
                var queryable = allBills.AsQueryable();

                // Filters
                queryable = ApplyFilters(queryable, request);

                // Total before paging
                var totalCount = queryable.Count();

                // Sorting
                queryable = ApplySorting(queryable, request.SortBy, request.SortDirection);

                // Paging
                var pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
                var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

                var page = queryable
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Map to DTO
                var dtoList = new List<BillDto>(page.Count);
                foreach (var bill in page)
                {
                    var dto = await _billMapper.MapAsync(bill, cancellationToken);
                    dtoList.Add(dto);
                }

                // Build paginated payload of BillDto (ONLY)
                var result = new PaginatedResult<BillDto>
                {
                    Items = dtoList,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                };

                return ApplicationResult<PaginatedResult<BillDto>>.Success(result, "Bills retrieved.");
            }
            catch (Exception ex)
            {
                return ApplicationResult<PaginatedResult<BillDto>>.Failure(ex, "Failed to retrieve user bills.");
            }
        }

        private static IQueryable<Bill> ApplyFilters(IQueryable<Bill> q, ListUserBillsByUserIdQuery request)
        {
            // Status
            if (!string.IsNullOrWhiteSpace(request.Status) &&
                Enum.TryParse<BillStatus>(request.Status, true, out var status))
            {
                q = q.Where(b => b.Status == status);
            }

            // Type
            if (!string.IsNullOrWhiteSpace(request.BillType))
            {
                q = q.Where(b => b.BillType.Equals(request.BillType, StringComparison.OrdinalIgnoreCase));
            }

            // Only overdue
            if (request.OnlyOverdue)
            {
                q = q.Where(b =>
                    b.DueDate.HasValue &&
                    b.DueDate.Value < DateTime.UtcNow &&
                    b.Status != BillStatus.FullyPaid &&
                    b.Status != BillStatus.Cancelled);
            }

            // Only unpaid (not fully paid and not cancelled)
            if (request.OnlyUnpaid)
            {
                q = q.Where(b => b.Status != BillStatus.FullyPaid && b.Status != BillStatus.Cancelled);
            }

            return q;
        }

        private static IQueryable<Bill> ApplySorting(IQueryable<Bill> q, string sortBy, string sortDirection)
        {
            var desc = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
            switch (sortBy?.Trim().ToLowerInvariant())
            {
                case "issuedate":
                    q = desc ? q.OrderByDescending(b => b.IssueDate) : q.OrderBy(b => b.IssueDate);
                    break;

                case "duedate":
                    q = desc ? q.OrderByDescending(b => b.DueDate) : q.OrderBy(b => b.DueDate);
                    break;

                case "totalamount":
                    q = desc ? q.OrderByDescending(b => b.TotalAmount.AmountRials) : q.OrderBy(b => b.TotalAmount.AmountRials);
                    break;

                case "status":
                    q = desc ? q.OrderByDescending(b => b.Status) : q.OrderBy(b => b.Status);
                    break;

                default:
                    q = desc ? q.OrderByDescending(b => b.IssueDate) : q.OrderBy(b => b.IssueDate);
                    break;
            }

            return q;
        }
    }
}
