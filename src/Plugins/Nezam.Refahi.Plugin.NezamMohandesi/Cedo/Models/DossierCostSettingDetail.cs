namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DossierCostSettingDetail
{
    public int Id { get; set; }

    public int DossierCostSettingId { get; set; }

    public int TariffDetailItemId { get; set; }

    public double PercentDiscount { get; set; }

    public double MertajeDiscount { get; set; }

    public virtual DossierCostSetting DossierCostSetting { get; set; } = null!;

    public virtual EngServiceTariffDetailItem TariffDetailItem { get; set; } = null!;
}
