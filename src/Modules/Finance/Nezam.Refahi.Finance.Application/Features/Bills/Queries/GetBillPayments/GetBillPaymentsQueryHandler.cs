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
using Nezam.Refahi.Finance.Application.Features.Payments.Queries.GetBillPayments;
using Nezam.Refahi.Finance.Application.Mappers;
using Nezam.Refahi.Finance.Application.Queries.Bills;
using Nezam.Refahi.Finance.Application.Spesifications;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Finance.Domain.Repositories;
using Nezam.Refahi.Shared.Application.Common.Models;

namespace Nezam.Refahi.Finance.Application.Features.Bills.Queries.GetUserBills
{
    /// <summary>
    /// Handler returns ONLY BillDto (no custom response wrapper), paginated.
    /// </summary>
    public sealed class GetBillPaymentsQueryHandler
        : IRequestHandler<GetBillPaymentsQuery, ApplicationResult<PaginatedResult<PaymentDto>>>
    {
        private readonly IPaymentRepository _billRepository;
        private readonly IValidator<GetBillPaymentsQuery> _validator;
        private readonly IMapper<Payment, PaymentDto> _billMapper;

        public GetBillPaymentsQueryHandler(
          IPaymentRepository billRepository,
            IValidator<GetBillPaymentsQuery> validator,
            IMapper<Payment, PaymentDto> billMapper)
        {
            _billRepository = billRepository ?? throw new ArgumentNullException(nameof(billRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _billMapper = billMapper ?? throw new ArgumentNullException(nameof(billMapper));
        }

        public async Task<ApplicationResult<PaginatedResult<PaymentDto>>> Handle(
          GetBillPaymentsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate
                var validation = await _validator.ValidateAsync(request, cancellationToken);
                if (!validation.IsValid)
                {
                    var errors = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    return ApplicationResult<PaginatedResult<PaymentDto>>.Failure(errors, "Validation failed.");
                }

                // Load all user's bills (repository should be optimized/indexed by ExternalUserId)
                var allBills = await _billRepository.GetPaginatedAsync(new GetBillPaymentsSpec(request.BillId,request.PageNumber,request.PageSize), cancellationToken);
                // Total before paging
                var totalCount = allBills.TotalCount;


                // Map to DTO
                var dtoList = new List<PaymentDto>(allBills.Items.Count());
                foreach (var bill in allBills.Items)
                {
                    var dto = await _billMapper.MapAsync(bill, cancellationToken);
                    dtoList.Add(dto);
                }

                // Build paginated payload of BillDto (ONLY)
                var result = new PaginatedResult<PaymentDto>
                {
                    Items = dtoList,
                    TotalCount = totalCount,
                    PageNumber = allBills.PageNumber,
                    PageSize = allBills.PageSize,
                };

                return ApplicationResult<PaginatedResult<PaymentDto>>.Success(result, "Bills retrieved.");
            }
            catch (Exception ex)
            {
                return ApplicationResult<PaginatedResult<PaymentDto>>.Failure(ex, "Failed to retrieve user bills.");
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
                q = q.Where(b => b.ReferenceType.Equals(request.BillType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
              q = q.Where(x=>x.ReferenceId.Contains(request.SearchTerm) || x.ReferenceType.Contains(request.SearchTerm) || x.ReferenceId.Contains(request.SearchTerm));
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
