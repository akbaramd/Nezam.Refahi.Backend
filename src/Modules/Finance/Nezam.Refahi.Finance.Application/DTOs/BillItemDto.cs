using MCA.SharedKernel.Application.Dtos;

namespace Nezam.Refahi.Finance.Application.DTOs;

/// <summary>
/// Bill item data transfer object
/// </summary>
public class BillItemDto : EntityDto<Guid>
{
    public Guid BillId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPriceRials { get; set; }
    public int Quantity { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal LineTotalRials { get; set; }
    public DateTime CreatedAt { get; set; }

    // Calculated fields
    public decimal SubtotalRials { get; set; }
    public decimal DiscountAmountRials { get; set; }
    public decimal TotalAmountRials { get; set; }
}

/// <summary>
/// Bill item creation request DTO
/// </summary>
public class CreateBillItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPriceRials { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal? DiscountPercentage { get; set; }
}

/// <summary>
/// Bill item update request DTO
/// </summary>
public class UpdateBillItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPriceRials { get; set; }
    public int Quantity { get; set; }
    public decimal? DiscountPercentage { get; set; }
}
