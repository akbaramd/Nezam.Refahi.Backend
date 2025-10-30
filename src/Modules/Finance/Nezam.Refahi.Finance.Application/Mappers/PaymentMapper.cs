using MCA.SharedKernel.Application.Contracts;
using Nezam.Refahi.Finance.Application.DTOs;
using Nezam.Refahi.Finance.Domain.Entities;

namespace Nezam.Refahi.Finance.Application.Mappers;

  public sealed class PaymentMapper : IMapper<Payment, PaymentDto>
    {
        public Task<PaymentDto> MapAsync(Payment source, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (source is null) throw new ArgumentNullException(nameof(source));
            var dto = PaymentDto.FromEntity(source, nowUtc: DateTime.UtcNow);
            return Task.FromResult(dto);
        }

        public Task MapAsync(Payment source, PaymentDto destination, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (destination is null) throw new ArgumentNullException(nameof(destination));

            var dto = PaymentDto.FromEntity(source, nowUtc: DateTime.UtcNow);
            CopyInto(dto, destination);
            return Task.CompletedTask;
        }

        private static void CopyInto(PaymentDto src, PaymentDto dst)
        {
            dst.PaymentId           = src.PaymentId;
            dst.BillId              = src.BillId;
            dst.AmountRials         = src.AmountRials;
            dst.Method              = src.Method;
            dst.Status              = src.Status;
            dst.StatusText          = src.StatusText;
            dst.GatewayText          = src.GatewayText;
            dst.MethodText          = src.MethodText;
            dst.CreatedAt           = src.CreatedAt;
            dst.CompletedAt         = src.CompletedAt;
            dst.GatewayReference      = src.GatewayReference;
            dst.Gateway             = src.Gateway;
            dst.GatewayTransactionId= src.GatewayTransactionId;
            dst.SecondsUntilExpiry  = src.SecondsUntilExpiry;
        }
    }

    // =========================================================
    // Payment -> PaymentDetailDto (full detail incl. transactions)
    // =========================================================
    public sealed class PaymentDetailMapper : IMapper<Payment, PaymentDetailDto>
    {
        public Task<PaymentDetailDto> MapAsync(Payment source, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (source is null) throw new ArgumentNullException(nameof(source));
            var dto = PaymentDetailDto.FromEntity(source, nowUtc: DateTime.UtcNow);
            return Task.FromResult(dto);
        }

        public Task MapAsync(Payment source, PaymentDetailDto destination, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (source is null) throw new ArgumentNullException(nameof(source));
            if (destination is null) throw new ArgumentNullException(nameof(destination));

            var dto = PaymentDetailDto.FromEntity(source, nowUtc: DateTime.UtcNow);
            CopyInto(dto, destination);
            return Task.CompletedTask;
        }

        private static void CopyInto(PaymentDetailDto src, PaymentDetailDto dst)
        {
            // Base PaymentDto fields
            dst.PaymentId           = src.PaymentId;
            dst.BillId              = src.BillId;
            dst.AmountRials         = src.AmountRials;
            dst.Method              = src.Method;
            dst.Status              = src.Status;
            dst.StatusText          = src.StatusText;
            dst.GatewayText          = src.GatewayText;
            dst.MethodText          = src.MethodText;
            dst.CreatedAt           = src.CreatedAt;
            dst.CompletedAt         = src.CompletedAt;
            dst.GatewayReference      = src.GatewayReference;
            dst.Gateway             = src.Gateway;
            dst.GatewayTransactionId= src.GatewayTransactionId;
            dst.SecondsUntilExpiry  = src.SecondsUntilExpiry;

            // Details
            dst.GatewayReference          = src.GatewayReference;
            dst.ExpiryDate                = src.ExpiryDate;
            dst.FailureReason             = src.FailureReason;
            dst.AppliedDiscountCode       = src.AppliedDiscountCode;
            dst.AppliedDiscountCodeId     = src.AppliedDiscountCodeId;
            dst.AppliedDiscountAmountRials= src.AppliedDiscountAmountRials;
            dst.IsFreePayment             = src.IsFreePayment;

            // Transactions (atomic replace)
            dst.Transactions = src.Transactions?.ToArray() ?? Array.Empty<PaymentTransactionDto>();
        }
    }
