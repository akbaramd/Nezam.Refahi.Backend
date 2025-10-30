using System;
using System.Collections.Generic;
using System.Linq;

using MCA.SharedKernel.Application.Dtos;
using Nezam.Refahi.Finance.Domain.Entities;
using Nezam.Refahi.Finance.Domain.Enums;
using Nezam.Refahi.Shared.Domain.ValueObjects;

namespace Nezam.Refahi.Finance.Application.DTOs;

  // =========================================================
  // Bill (List/Raw) – NO relations. Derived fields included.
  // =========================================================

  /// <summary>
  /// Canonical Bill DTO for listings and raw reads. No relations are included.
  /// Contains core identity, subject, state, amounts, dates, metadata,
  /// and light aggregates (counts/percentages) but NO child collections.
  /// </summary>
  public  class BillDto : DeletableAggregateDto<Guid>
  {
      // Identity
      public string BillNumber { get; set; } = string.Empty;
      public string Title { get; set; } = string.Empty;
      public string ReferenceTrackingCode { get; set; } = string.Empty;
      public string ReferenceId { get; set; } = string.Empty;
      public string ReferenceType { get; set; } = string.Empty;

      // Subject
      public Guid ExternalUserId { get; set; }
      public string? UserFullName { get; set; }

      // State (stringified enums + localized text)
      public string Status { get; set; } = string.Empty;  // e.g., Draft, Issued, PartiallyPaid, FullyPaid, Overdue, Cancelled, ...
      public string StatusText { get; set; } = string.Empty; // Persian localized label

      // Amounts (Rials)
      public decimal TotalAmountRials { get; set; }
      public decimal PaidAmountRials { get; set; }
      public decimal RemainingAmountRials { get; set; }

      // Discounts (flat values only)
      public decimal? DiscountAmountRials { get; set; }
      public string? DiscountCode { get; set; }
      public Guid? DiscountCodeId { get; set; }

      // Dates
      public DateTime IssueDate { get; set; }
      public DateTime? DueDate { get; set; }
      public DateTime? FullyPaidDate { get; set; }


      // Meta
      public string? Description { get; set; }
      public Dictionary<string, string> Metadata { get; set; } = new();

      // Derived/summary
      public bool IsPaid { get; set; }
      public bool IsPartiallyPaid { get; set; }
      public bool IsOverdue { get; set; }
      public bool IsCancelled { get; set; }

      /// <summary>0..100</summary>
      public decimal PaymentCompletionPercentage { get; set; }

      /// <summary>Seconds until due date. Negative if passed or null if no due date.</summary>
      public long? SecondsUntilDue { get; set; }

      /// <summary>Seconds overdue. 0 if not overdue.</summary>
      public long SecondsOverdue { get; set; }

      // Aggregates (counts only; relations live in BillDetailsDto)
      public int ItemsCount { get; set; }
      public int PaymentsCount { get; set; }
      public int RefundsCount { get; set; }

      // ---- Factory ----
      public static BillDto FromEntity(Bill entity, DateTime? nowUtc = null)
      {
          if (entity == null) throw new ArgumentNullException(nameof(entity));
          var now = nowUtc ?? DateTime.UtcNow;

          var total = entity.TotalAmount?.AmountRials ?? 0m;
          var paid = entity.PaidAmount?.AmountRials ?? 0m;
          var remaining = entity.RemainingAmount?.AmountRials ?? Math.Max(0, total - paid);

          var due = entity.DueDate;
          long? secondsUntilDue = due.HasValue ? (long?)(due.Value - now).TotalSeconds : null;
          var overdue = entity.IsOverdue();
          long secondsOverdue = overdue && due.HasValue ? Math.Max(0, (long)(now - due.Value).TotalSeconds) : 0;

          var statusStr = entity.Status.ToString();
          var statusFa = PersianText.For(entity.Status);

          // counts (navigation collections are available; do not load heavy graphs here)
          var itemsCount = entity.Items?.Count ?? 0;
          var paymentsCount = entity.Payments?.Count ?? 0;
          var refundsCount = entity.Refunds?.Count ?? 0;

          var discountAmount = entity.DiscountAmount?.AmountRials;

          return new BillDto
          {
              Id = entity.Id,
              CreatedAt = entity.CreatedAt,
              LastModifiedAt = entity.LastModifiedAt,
              DeletedAt = entity.DeletedAt,

              BillNumber = entity.BillNumber,
              Title = entity.Title,
              ReferenceTrackingCode = entity.ReferenceTrackCode,
              ReferenceId = entity.ReferenceId,
              ReferenceType = entity.ReferenceType,

              ExternalUserId = entity.ExternalUserId,
              UserFullName = entity.UserFullName,

              Status = statusStr,
              StatusText = statusFa,

              TotalAmountRials = total,
              PaidAmountRials = paid,
              RemainingAmountRials = remaining,

              DiscountAmountRials = discountAmount,
              DiscountCode = entity.DiscountCode,
              DiscountCodeId = entity.DiscountCodeId,

              IssueDate = entity.IssueDate,
              DueDate = entity.DueDate,
              FullyPaidDate = entity.FullyPaidDate,

           
              Description = entity.Description,
              Metadata = new Dictionary<string, string>(entity.Metadata ?? new()),

              IsPaid = entity.Status == BillStatus.FullyPaid,
              IsPartiallyPaid = entity.Status == BillStatus.PartiallyPaid,
              IsOverdue = overdue,
              IsCancelled = entity.Status == BillStatus.Cancelled,

              PaymentCompletionPercentage = total <= 0 ? (paid > 0 ? 100 : 0) : Math.Round((paid / total) * 100m, 2),
              SecondsUntilDue = secondsUntilDue,
              SecondsOverdue = secondsOverdue,

              ItemsCount = itemsCount,
              PaymentsCount = paymentsCount,
              RefundsCount = refundsCount
          };
      }
  }

  // =========================================================
  // Bill (Details) – INCLUDES relations (lightweight DTOs).
  // =========================================================

  /// <summary>
  /// Bill details DTO: extends BillDto and adds relation DTO collections.
  /// </summary>
  public  class BillDetailDto : BillDto
  {
      public IReadOnlyList<BillItemDto> Items { get; set; } = Array.Empty<BillItemDto>();
      public IReadOnlyList<PaymentDto> Payments { get; set; } = Array.Empty<PaymentDto>();
      public IReadOnlyList<RefundDto> Refunds { get; set; } = Array.Empty<RefundDto>();

      // ---- Factory ----
      /// <summary>
      /// Map Bill to BillDetailDto. Relations are populated; if you need the full payment
      /// details with transactions use PaymentDetailDto separately.
      /// </summary>
      public static BillDetailDto FromEntity(
          Bill entity,
          bool includeItems = true,
          bool includePayments = true,
          bool includeRefunds = true,
          DateTime? nowUtc = null)
      {
          if (entity == null) throw new ArgumentNullException(nameof(entity));

          var baseDto = BillDto.FromEntity(entity, nowUtc);

          return new BillDetailDto
          {
              // inherit base
              Id = baseDto.Id,
              CreatedAt = baseDto.CreatedAt,
              LastModifiedAt = baseDto.LastModifiedAt,
              DeletedAt = baseDto.DeletedAt,

              BillNumber = baseDto.BillNumber,
              Title = baseDto.Title,
              ReferenceId = baseDto.ReferenceId,
              ReferenceTrackingCode = baseDto.ReferenceTrackingCode,
              ReferenceType = baseDto.ReferenceType,

              ExternalUserId = baseDto.ExternalUserId,
              UserFullName = baseDto.UserFullName,

              Status = baseDto.Status,
              StatusText = baseDto.StatusText,

              TotalAmountRials = baseDto.TotalAmountRials,
              PaidAmountRials = baseDto.PaidAmountRials,
              RemainingAmountRials = baseDto.RemainingAmountRials,

              DiscountAmountRials = baseDto.DiscountAmountRials,
              DiscountCode = baseDto.DiscountCode,
              DiscountCodeId = baseDto.DiscountCodeId,

              IssueDate = baseDto.IssueDate,
              DueDate = baseDto.DueDate,
              FullyPaidDate = baseDto.FullyPaidDate,
              Description = baseDto.Description,
              Metadata = baseDto.Metadata,

              IsPaid = baseDto.IsPaid,
              IsPartiallyPaid = baseDto.IsPartiallyPaid,
              IsOverdue = baseDto.IsOverdue,
              IsCancelled = baseDto.IsCancelled,

              PaymentCompletionPercentage = baseDto.PaymentCompletionPercentage,
              SecondsUntilDue = baseDto.SecondsUntilDue,
              SecondsOverdue = baseDto.SecondsOverdue,

              ItemsCount = baseDto.ItemsCount,
              PaymentsCount = baseDto.PaymentsCount,
              RefundsCount = baseDto.RefundsCount,

              // relations (lightweight)
              Items = includeItems
                  ? (entity.Items ?? Array.Empty<BillItem>())
                      .Select(BillItemDto.FromEntity)
                      .ToArray()
                  : Array.Empty<BillItemDto>(),

              Payments = includePayments
                  ? (entity.Payments ?? Array.Empty<Payment>())
                      .Select(p => PaymentDto.FromEntity(p))
                      .OrderByDescending(p => p.CreatedAt)
                      .ToArray()
                  : Array.Empty<PaymentDto>(),

              Refunds = includeRefunds
                  ? (entity.Refunds ?? Array.Empty<Refund>())
                      .Select(r => RefundDto.FromEntity(r))
                      .OrderByDescending(r => r.RequestedAt)
                      .ToArray()
                  : Array.Empty<RefundDto>()
          };
      }
  }

  // =========================================================
  // Bill Item (lightweight)
  // =========================================================

  /// <summary>
  /// Line item DTO (lightweight; no back-references).
  /// </summary>
  public  record BillItemDto
  {
      public Guid ItemId { get; set; }
      public string Title { get; set; } = string.Empty;
      public string? Description { get; set; }
      public decimal UnitPriceRials { get; set; }
      public int Quantity { get; set; }
      public decimal? DiscountPercentage { get; set; }
      public decimal LineTotalRials { get; set; }
      public DateTime CreatedAt { get; set; }

      public static BillItemDto FromEntity(BillItem entity)
      {
          if (entity == null) throw new ArgumentNullException(nameof(entity));
          return new BillItemDto
          {
              ItemId = entity.Id,
              Title = entity.Title,
              Description = entity.Description,
              UnitPriceRials = entity.UnitPrice?.AmountRials ?? 0m,
              Quantity = entity.Quantity,
              DiscountPercentage = entity.DiscountPercentage,
              LineTotalRials = entity.LineTotal?.AmountRials ?? 0m,
              CreatedAt = entity.CreatedAt
          };
      }
  }

  // =========================================================
  // Payment – lightweight
  // =========================================================

  /// <summary>
  /// Lightweight payment DTO for listings and embedded views.
  /// </summary>
  public  record PaymentDto
  {
      public Guid PaymentId { get; set; }
      public Guid BillId { get; set; }

      /// <summary>Amount in rials.</summary>
      public decimal AmountRials { get; set; }


      /// <summary>Pending, Processing, Completed, Failed, Cancelled, Expired, ...</summary>
      public string Status { get; set; } = string.Empty;

      /// <summary>Localized Persian status.</summary>
      public string MethodText { get; set; } = string.Empty;
      
      
      /// <summary>Localized Persian status.</summary>
      public string GatewayText { get; set; } = string.Empty;
      
      public string Method { get; set; } = string.Empty;

      /// <summary>Localized Persian status.</summary>
      public string StatusText { get; set; } = string.Empty;

      public DateTime CreatedAt { get; set; }
      public DateTime? CompletedAt { get; set; }

      /// <summary>Business tracking number exposed to user (nullable).</summary>

      /// <summary>Gateway name/code when applicable.</summary>
      public string? Gateway { get; set; }

      /// <summary>Final transaction id from gateway (nullable until completion).</summary>
      public string? GatewayTransactionId { get; set; }
      public string? GatewayReference { get; set; }

      /// <summary>Seconds until expiry. Null if no expiry; 0 if expired; positive if active.</summary>
      public long? SecondsUntilExpiry { get; set; }

      public static PaymentDto FromEntity(Payment entity, DateTime? nowUtc = null)
      {
          if (entity == null) throw new ArgumentNullException(nameof(entity));
          var now = nowUtc ?? DateTime.UtcNow;

          long? secondsUntilExpiry = entity.ExpiryDate.HasValue
              ? Math.Max(0, (long)(entity.ExpiryDate.Value - now).TotalSeconds)
              : (long?)null;

          return new PaymentDto
          {
              PaymentId = entity.Id,
              BillId = entity.BillId,
              AmountRials = entity.Amount?.AmountRials ?? 0m,
              Method = entity.Method.ToString(),
              Status = entity.Status.ToString(),
              StatusText = PersianText.For(entity.Status),
              CreatedAt = entity.CreatedAt,
              CompletedAt = entity.CompletedAt,
              Gateway = entity.Gateway?.ToString(),
              GatewayText = PaymentGatewayTitles.For(entity.Gateway),
              MethodText = PaymentMethodTitles.For(entity.Method),
              GatewayTransactionId = entity.GatewayTransactionId,
              GatewayReference = entity.GatewayReference,
              SecondsUntilExpiry = secondsUntilExpiry
          };
      }
  }

  // =========================================================
  // Payment – details (adds gateway refs, expiry, failure, transactions)
  // =========================================================

  public  record PaymentDetailDto : PaymentDto
  {
      public DateTime? ExpiryDate { get; set; }
      public string? FailureReason { get; set; }

      public string? AppliedDiscountCode { get; set; }
      public Guid? AppliedDiscountCodeId { get; set; }
      public decimal? AppliedDiscountAmountRials { get; set; }
      public bool IsFreePayment { get; set; }
      public required BillDto Bill { get; set; }
      public IReadOnlyList<PaymentTransactionDto> Transactions { get; set; } = Array.Empty<PaymentTransactionDto>();

      public new static PaymentDetailDto FromEntity(Payment entity, DateTime? nowUtc = null)
      {
          if (entity == null) throw new ArgumentNullException(nameof(entity));
          var baseDto = PaymentDto.FromEntity(entity, nowUtc);

          var transactions = entity.Transactions
              .OrderBy(t => t.CreatedAt)
              .Select(t => PaymentTransactionDto.FromEntity(t))
              .ToArray();

          var bill = BillDto.FromEntity(entity.Bill);
          return new PaymentDetailDto
          {
              // base
              PaymentId = baseDto.PaymentId,
              BillId = baseDto.BillId,
              AmountRials = baseDto.AmountRials,
              Method = baseDto.Method,
              Status = baseDto.Status,
              StatusText = baseDto.StatusText,
              CreatedAt = baseDto.CreatedAt,
              CompletedAt = baseDto.CompletedAt,
              Gateway = baseDto.Gateway,
              GatewayTransactionId = baseDto.GatewayTransactionId,
              SecondsUntilExpiry = baseDto.SecondsUntilExpiry,

              // details
              GatewayReference = entity.GatewayReference,
              ExpiryDate = entity.ExpiryDate,
              FailureReason = entity.FailureReason,
              AppliedDiscountCode = entity.AppliedDiscountCode,
              AppliedDiscountCodeId = entity.AppliedDiscountCodeId,
              AppliedDiscountAmountRials = entity.AppliedDiscountAmount?.AmountRials,
              IsFreePayment = entity.IsFreePayment,
              Transactions = transactions,
              GatewayText = PaymentGatewayTitles.For(entity.Gateway),
              MethodText = PaymentMethodTitles.For(entity.Method),
              Bill = bill
          };
      }
  }

  public  record PaymentTransactionDto
  {
      public Guid TransactionId { get; set; }
      public decimal? AmountRials { get; set; }
      public DateTime CreatedAt { get; set; }
      public string? Gateway { get; set; }
      public string? GatewayTransactionId { get; set; }
      public string? GatewayReference { get; set; }
      public string Status { get; set; } = string.Empty;
      public string StatusText { get; set; } = string.Empty;
      public string? Note { get; set; }

      public static PaymentTransactionDto FromEntity(PaymentTransaction entity)
      {
          if (entity == null) throw new ArgumentNullException(nameof(entity));
          return new PaymentTransactionDto
          {
              TransactionId = entity.Id,
              AmountRials = entity.ProcessedAmount?.AmountRials,
              CreatedAt = entity.CreatedAt,
              Gateway = entity.GatewayReference?.ToString(),
              GatewayTransactionId = entity.GatewayTransactionId,
              GatewayReference = entity.GatewayReference,
              Status = entity.Status.ToString(),
              StatusText = PersianText.For(entity.Status),
          };
      }
  }

  // =========================================================
  // Refund – lightweight (includes processing timestamps)
  // =========================================================

  public  record RefundDto
  {
      public Guid RefundId { get; set; }
      public Guid BillId { get; set; }
      public decimal AmountRials { get; set; }
      public string Status { get; set; } = string.Empty;     // Pending, Processing, Completed, Rejected, Failed
      public string StatusText { get; set; } = string.Empty; // Persian localized
      public string Reason { get; set; } = string.Empty;
      public Guid RequestedByExternalUserId { get; set; }
      public DateTime RequestedAt { get; set; }
      public DateTime? ProcessedAt { get; set; }
      public DateTime? CompletedAt { get; set; }
      public string? GatewayRefundId { get; set; }
      public string? GatewayReference { get; set; }
      public string? ProcessorNotes { get; set; }
      public string? RejectionReason { get; set; }

      /// <summary>Seconds since request to now (age). Useful for SLAs.</summary>
      public long SecondsSinceRequested { get; set; }

      public static RefundDto FromEntity(Refund entity, DateTime? nowUtc = null)
      {
          if (entity == null) throw new ArgumentNullException(nameof(entity));
          var now = nowUtc ?? DateTime.UtcNow;

          var statusStr = entity.Status.ToString();
          var statusFa = PersianText.For(entity.Status);

          var secondsSinceRequested = Math.Max(0, (long)(now - entity.RequestedAt).TotalSeconds);

          return new RefundDto
          {
              RefundId = entity.Id,
              BillId = entity.BillId,
              AmountRials = entity.Amount?.AmountRials ?? 0m,
              Status = statusStr,
              StatusText = statusFa,
              Reason = entity.Reason,
              RequestedByExternalUserId = entity.RequestedByExternalUserId,
              RequestedAt = entity.RequestedAt,
              ProcessedAt = entity.ProcessedAt,
              CompletedAt = entity.CompletedAt,
              GatewayRefundId = entity.GatewayRefundId,
              GatewayReference = entity.GatewayReference,
              ProcessorNotes = entity.ProcessorNotes,
              RejectionReason = entity.RejectionReason,
              SecondsSinceRequested = secondsSinceRequested
          };
      }
  }

  // =========================================================
  // Persian Status Localizer (no reflection, no magic)
  // =========================================================

  internal static class PersianText
  {
      public static string For(BillStatus status) => status switch
      {
          BillStatus.Draft         => "پیش‌نویس",
          BillStatus.Issued        => "صادر شده",
          BillStatus.PartiallyPaid => "نیمه‌پرداخت",
          BillStatus.FullyPaid     => "تسویه کامل",
          BillStatus.Overdue       => "سررسید گذشته",
          BillStatus.Cancelled     => "لغو شده",
          BillStatus.Voided        => "باطل شده",
          BillStatus.WrittenOff    => "سوخت شده",
          BillStatus.Credited      => "اعتبار صادر شده",
          BillStatus.Disputed      => "مورد اعتراض",
          BillStatus.Refunded      => "مسترد شده",
          _                        => status.ToString()
      };

      public static string For(PaymentStatus status) => status switch
      {
          PaymentStatus.Pending    => "در انتظار",
          PaymentStatus.Processing => "در حال پردازش",
          PaymentStatus.Completed  => "موفق",
          PaymentStatus.Failed     => "ناموفق",
          PaymentStatus.Cancelled  => "لغو شده",
          PaymentStatus.Expired    => "منقضی",
          _                        => status.ToString()
      };

      public static string For(RefundStatus status) => status switch
      {
          RefundStatus.Pending    => "در انتظار",
          RefundStatus.Processing => "در حال پردازش",
          RefundStatus.Completed  => "تکمیل شده",
          RefundStatus.Rejected   => "رد شده",
          RefundStatus.Failed     => "ناموفق",
          _                       => status.ToString()
      };
  }

  public static class PaymentGatewayTitles
  {
    public static string For(PaymentGateway? gateway) => gateway switch
    {
      PaymentGateway.Zarinpal  => "زرین‌پال",
      PaymentGateway.Mellat    => "بانک ملت",
      PaymentGateway.Parsian   => "بانک پارسیان",
      PaymentGateway.Saman     => "بانک سامان",
      PaymentGateway.Pasargad  => "بانک پاسارگاد",
      PaymentGateway.System    => "سیستم",
      null                     => "—",
      _                        => gateway.ToString() ?? "نامشخص"
    };
  }

  public static class PaymentMethodTitles
  {
    public static string For(PaymentMethod method) => method switch
    {
      PaymentMethod.Online       => "پرداخت آنلاین",
      PaymentMethod.BankTransfer => "انتقال بانکی",
      PaymentMethod.Cash         => "نقدی",
      PaymentMethod.CardToCard   => "کارت به کارت",
      PaymentMethod.Wallet       => "کیف پول",
      _                          => method.ToString()
    };
  }

  