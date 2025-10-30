using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Application.Mappers
{
    /// <summary>
    /// Maps Bill aggregate to BillDto (list/raw view; no relations).
    /// </summary>
    public sealed class BillMapper : IMapper<Bill, BillDto>
    {
        public Task<BillDto> MapAsync(Bill source, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (source is null) throw new ArgumentNullException(nameof(source));

            // Single source of truth for projection logic.
            var dto = BillDto.FromEntity(source, nowUtc: DateTime.UtcNow);
            return Task.FromResult(dto);
        }

        public Task MapAsync(Bill source, BillDto destination, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (destination is null) throw new ArgumentNullException(nameof(destination));

            var dto = BillDto.FromEntity(source, nowUtc: DateTime.UtcNow);
            CopyInto(dto, destination);
            return Task.CompletedTask;
        }

        private static void CopyInto(BillDto src, BillDto dst)
        {
            // Base audit (DeletableAggregateDto)
            dst.Id             = src.Id;
            dst.CreatedAt      = src.CreatedAt;
            dst.LastModifiedAt = src.LastModifiedAt;
            dst.DeletedAt      = src.DeletedAt;

            // Identity
            dst.BillNumber  = src.BillNumber;
            dst.Title       = src.Title;
            dst.ReferenceId = src.ReferenceId;
            dst.ReferenceTrackingCode = src.ReferenceTrackingCode;
            dst.ReferenceType    = src.ReferenceType;

            // Subject
            dst.ExternalUserId = src.ExternalUserId;
            dst.UserFullName   = src.UserFullName;

            // State
            dst.Status     = src.Status;
            dst.StatusText = src.StatusText;

            // Amounts
            dst.TotalAmountRials     = src.TotalAmountRials;
            dst.PaidAmountRials      = src.PaidAmountRials;
            dst.RemainingAmountRials = src.RemainingAmountRials;

            // Discount
            dst.DiscountAmountRials = src.DiscountAmountRials;
            dst.DiscountCode        = src.DiscountCode;
            dst.DiscountCodeId      = src.DiscountCodeId;

            // Dates
            dst.IssueDate     = src.IssueDate;
            dst.DueDate       = src.DueDate;
            dst.FullyPaidDate = src.FullyPaidDate;


            // Meta
            dst.Description = src.Description;
            dst.Metadata    = new System.Collections.Generic.Dictionary<string, string>(src.Metadata);

            // Derived
            dst.IsPaid                      = src.IsPaid;
            dst.IsPartiallyPaid             = src.IsPartiallyPaid;
            dst.IsOverdue                   = src.IsOverdue;
            dst.IsCancelled                 = src.IsCancelled;
            dst.PaymentCompletionPercentage = src.PaymentCompletionPercentage;
            dst.SecondsUntilDue             = src.SecondsUntilDue;
            dst.SecondsOverdue              = src.SecondsOverdue;

            // Aggregates
            dst.ItemsCount    = src.ItemsCount;
            dst.PaymentsCount = src.PaymentsCount;
            dst.RefundsCount  = src.RefundsCount;
        }
    }

    /// <summary>
    /// Maps Bill aggregate to BillDetailDto (details view; relations always included).
    /// No include arguments; always performs full mapping.
    /// </summary>
    public sealed class BillDetailMapper : IMapper<Bill, BillDetailDto>
    {
        public Task<BillDetailDto> MapAsync(Bill source, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (source is null) throw new ArgumentNullException(nameof(source));

            var dto = BuildDetail(source, nowUtc: DateTime.UtcNow);
            return Task.FromResult(dto);
        }

        public Task MapAsync(Bill source, BillDetailDto destination, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (destination is null) throw new ArgumentNullException(nameof(destination));

            var dto = BuildDetail(source, nowUtc: DateTime.UtcNow);
            CopyInto(dto, destination);
            return Task.CompletedTask;
        }

        // —— helpers ——

        private static BillDetailDto BuildDetail(Bill source, DateTime? nowUtc)
        {
            // Base projection (single source of truth for core fields)
            var baseDto = BillDto.FromEntity(source, nowUtc);

            // Relations (full mapping; no flags)
            var items = (source.Items ?? Array.Empty<BillItem>())
                .Select(BillItemDto.FromEntity)
                .ToArray();

            var payments = (source.Payments ?? Array.Empty<Payment>())
                .Select(p => PaymentDto.FromEntity(p, nowUtc))
                .OrderByDescending(p => p.CreatedAt)
                .ToArray();

            var refunds = (source.Refunds ?? Array.Empty<Refund>())
                .Select(r => RefundDto.FromEntity(r, nowUtc))
                .OrderByDescending(r => r.RequestedAt)
                .ToArray();

            return new BillDetailDto
            {
                // base audit
                Id             = baseDto.Id,
                CreatedAt      = baseDto.CreatedAt,
                LastModifiedAt = baseDto.LastModifiedAt,
                DeletedAt      = baseDto.DeletedAt,

                // identity
                BillNumber  = baseDto.BillNumber,
                Title       = baseDto.Title,
                ReferenceId = baseDto.ReferenceId,
                ReferenceType    = baseDto.ReferenceType,
                ReferenceTrackingCode    = baseDto.ReferenceTrackingCode,

                // subject
                ExternalUserId = baseDto.ExternalUserId,
                UserFullName   = baseDto.UserFullName,

                // state
                Status     = baseDto.Status,
                StatusText = baseDto.StatusText,

                // amounts
                TotalAmountRials     = baseDto.TotalAmountRials,
                PaidAmountRials      = baseDto.PaidAmountRials,
                RemainingAmountRials = baseDto.RemainingAmountRials,

                // discount
                DiscountAmountRials = baseDto.DiscountAmountRials,
                DiscountCode        = baseDto.DiscountCode,
                DiscountCodeId      = baseDto.DiscountCodeId,

                // dates
                IssueDate     = baseDto.IssueDate,
                DueDate       = baseDto.DueDate,
                FullyPaidDate = baseDto.FullyPaidDate,

               
                // meta
                Description = baseDto.Description,
                Metadata    = baseDto.Metadata,

                // derived
                IsPaid                      = baseDto.IsPaid,
                IsPartiallyPaid             = baseDto.IsPartiallyPaid,
                IsOverdue                   = baseDto.IsOverdue,
                IsCancelled                 = baseDto.IsCancelled,
                PaymentCompletionPercentage = baseDto.PaymentCompletionPercentage,
                SecondsUntilDue             = baseDto.SecondsUntilDue,
                SecondsOverdue              = baseDto.SecondsOverdue,

                // aggregates
                ItemsCount    = baseDto.ItemsCount,
                PaymentsCount = baseDto.PaymentsCount,
                RefundsCount  = baseDto.RefundsCount,

                // relations
                Items    = items,
                Payments = payments,
                Refunds  = refunds
            };
        }

        private static void CopyInto(BillDetailDto src, BillDetailDto dst)
        {
            // Base (BillDto fields)
            dst.Id             = src.Id;
            dst.CreatedAt      = src.CreatedAt;
            dst.LastModifiedAt = src.LastModifiedAt;
            dst.DeletedAt      = src.DeletedAt;

            dst.BillNumber  = src.BillNumber;
            dst.Title       = src.Title;
            dst.ReferenceId = src.ReferenceId;
            dst.ReferenceType    = src.ReferenceType;

            dst.ExternalUserId = src.ExternalUserId;
            dst.UserFullName   = src.UserFullName;

            dst.Status     = src.Status;
            dst.StatusText = src.StatusText;

            dst.TotalAmountRials     = src.TotalAmountRials;
            dst.PaidAmountRials      = src.PaidAmountRials;
            dst.RemainingAmountRials = src.RemainingAmountRials;

            dst.DiscountAmountRials = src.DiscountAmountRials;
            dst.DiscountCode        = src.DiscountCode;
            dst.DiscountCodeId      = src.DiscountCodeId;

            dst.IssueDate     = src.IssueDate;
            dst.DueDate       = src.DueDate;
            dst.FullyPaidDate = src.FullyPaidDate;

            dst.ReferenceTrackingCode = src.ReferenceTrackingCode;

            dst.Description = src.Description;
            dst.Metadata    = src.Metadata;

            dst.IsPaid                      = src.IsPaid;
            dst.IsPartiallyPaid             = src.IsPartiallyPaid;
            dst.IsOverdue                   = src.IsOverdue;
            dst.IsCancelled                 = src.IsCancelled;
            dst.PaymentCompletionPercentage = src.PaymentCompletionPercentage;
            dst.SecondsUntilDue             = src.SecondsUntilDue;
            dst.SecondsOverdue              = src.SecondsOverdue;

            dst.ItemsCount    = src.ItemsCount;
            dst.PaymentsCount = src.PaymentsCount;
            dst.RefundsCount  = src.RefundsCount;

            // Relations (atomic replacement)
            dst.Items    = src.Items?.ToArray()    ?? Array.Empty<BillItemDto>();
            dst.Payments = src.Payments?.ToArray() ?? Array.Empty<PaymentDto>();
            dst.Refunds  = src.Refunds?.ToArray()  ?? Array.Empty<RefundDto>();
        }
    }
}
