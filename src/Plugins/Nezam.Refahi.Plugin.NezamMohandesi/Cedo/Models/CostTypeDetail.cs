namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CostTypeDetail
{
    public int Id { get; set; }

    public int CostTypeId { get; set; }

    public string? Title { get; set; }

    public bool IsActive { get; set; }

    public int? ServiceFieldId { get; set; }

    public bool IsCoordinatorPayment { get; set; }

    public virtual CostType CostType { get; set; } = null!;

    public virtual ICollection<CostTypeDetailBill> CostTypeDetailBills { get; set; } = new List<CostTypeDetailBill>();

    public virtual ICollection<CostTypeDetailSetting> CostTypeDetailSettings { get; set; } = new List<CostTypeDetailSetting>();

    public virtual ICollection<EngServiceTariffDetailItem> EngServiceTariffDetailItems { get; set; } = new List<EngServiceTariffDetailItem>();

    public virtual ServiceField? ServiceField { get; set; }
}
