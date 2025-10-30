namespace Nezam.Refahi.Finance.Application.DTOs;

public sealed record DiscountValidationDto
{
  // Outcome
  public bool IsValid { get; init; }
  public List<string> Errors { get; init; } = new();

  // Computed amounts
  public decimal DiscountAmountRials { get; init; }
  public decimal NewTotalAmountRials { get; init; }
  public decimal? DiscountPercentage { get; init; }
  public bool IsPercentageDiscount { get; init; }
  public bool IsFixedAmountDiscount { get; init; }

  // Bill snapshot
  public BillDiscountSnapshotDto Bill { get; init; } = new();

  // Discount code snapshot
  public DiscountCodeSnapshotDto DiscountCode { get; init; } = new();
}

public sealed record BillDiscountSnapshotDto
{
    public Guid BillId { get; init; }
    public string BillNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ReferenceId { get; init; } = string.Empty;
    public string BillType { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
    public string StatusText { get; init; } = string.Empty;

    public Guid ExternalUserId { get; init; }
    public string? UserFullName { get; init; }

    public decimal OriginalTotalAmountRials { get; init; }
    public decimal PaidAmountRials { get; init; }
    public decimal RemainingAmountRials { get; init; }

    public string? AppliedDiscountCode { get; init; }
    public Guid? AppliedDiscountCodeId { get; init; }
    public bool HasAppliedDiscount { get; init; }

    public DateTime IssueDate { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime? FullyPaidDate { get; init; }

    public bool IsPaid { get; init; }
    public bool IsPartiallyPaid { get; init; }
    public bool IsOverdue { get; init; }
    public bool IsCancelled { get; init; }
    public bool CanApplyDiscount { get; init; }

    public List<DiscountValidationItemDto> Items { get; init; } = new();
}

/// <summary>Discount code snapshot.</summary>
public sealed record DiscountCodeSnapshotDto
{
    public Guid DiscountCodeId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;   // enum as string
    public string Status { get; init; } = string.Empty; // enum as string

    public decimal Value { get; init; }
    public DateTime ValidFrom { get; init; }
    public DateTime ValidTo { get; init; }

    public int? UsageLimit { get; init; }
    public int CurrentUsages { get; init; }
    public int RemainingUsages { get; init; }
    public bool IsSingleUse { get; init; }

    public string? Description { get; init; }
    public decimal? MinimumBillAmountRials { get; init; }
    public decimal? MaximumDiscountAmountRials { get; init; }

    public bool IsExpired { get; init; }
    public bool IsDepleted { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>Compact bill item line for validation view.</summary>
public sealed record DiscountValidationItemDto
{
    public Guid ItemId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal UnitPriceRials { get; init; }
    public int Quantity { get; init; }
    public decimal LineTotalRials { get; init; }
    public string? ReferenceId { get; init; }
    public Dictionary<string, string> Metadata { get; init; } = new();
}