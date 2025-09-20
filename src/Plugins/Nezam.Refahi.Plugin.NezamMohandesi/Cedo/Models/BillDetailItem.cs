namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BillDetailItem
{
    public int Id { get; set; }

    public int BillDetailId { get; set; }

    public int TariffDetailItemId { get; set; }

    public long Amount { get; set; }

    public long Tax { get; set; }

    public long Tool { get; set; }

    public string? Description { get; set; }

    public long DiscountAmount { get; set; }

    public long TaxDiscount { get; set; }

    public long ToolDiscount { get; set; }

    public virtual BillDetail BillDetail { get; set; } = null!;

    public virtual EngServiceTariffDetailItem TariffDetailItem { get; set; } = null!;
}
