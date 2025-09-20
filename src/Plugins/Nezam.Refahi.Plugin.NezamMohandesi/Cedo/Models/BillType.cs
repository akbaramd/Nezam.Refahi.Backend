namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BillType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<CostTypeDetailBill> CostTypeDetailBills { get; set; } = new List<CostTypeDetailBill>();

    public virtual ICollection<DossierCostSetting> DossierCostSettings { get; set; } = new List<DossierCostSetting>();

    public virtual ICollection<EngServiceTariffDetailItem> EngServiceTariffDetailItems { get; set; } = new List<EngServiceTariffDetailItem>();

    public virtual ICollection<OnlinePaymentSetting> OnlinePaymentSettings { get; set; } = new List<OnlinePaymentSetting>();
}
