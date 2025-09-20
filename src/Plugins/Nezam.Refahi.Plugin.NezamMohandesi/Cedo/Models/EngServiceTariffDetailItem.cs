namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class EngServiceTariffDetailItem
{
    public int Id { get; set; }

    public int EngServiceTariffDetailId { get; set; }

    public int CostTypeDetailId { get; set; }

    public int BillTypeId { get; set; }

    public double Amount { get; set; }

    public bool CanDiscount { get; set; }

    public double TaxPercentage { get; set; }

    public double TollPercentage { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<BillDetailItem> BillDetailItems { get; set; } = new List<BillDetailItem>();

    public virtual BillType BillType { get; set; } = null!;

    public virtual CostTypeDetail CostTypeDetail { get; set; } = null!;

    public virtual ICollection<DossierCostSettingDetail> DossierCostSettingDetails { get; set; } = new List<DossierCostSettingDetail>();

    public virtual EngServiceTariffDetail EngServiceTariffDetail { get; set; } = null!;
}
