namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BillDetailItem
{
    public int Id { get; set; }

    public int BillDetailId { get; set; }

    public int TariffDetailItemId { get; set; }

    public decimal Amount { get; set; }

    public decimal Tax { get; set; }

    public decimal Tool { get; set; }

    public string? Description { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TaxDiscount { get; set; }

    public decimal ToolDiscount { get; set; }

    public virtual BillDetail BillDetail { get; set; } = null!;

    public virtual EngServiceTariffDetailItem TariffDetailItem { get; set; } = null!;
}
