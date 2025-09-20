namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class EngServiceTariffDetail
{
    public int Id { get; set; }

    public int EngServiceTariffId { get; set; }

    public int BuildingSubGroupId { get; set; }

    public double TariffAmount { get; set; }

    public double ExecuterAmount { get; set; }

    public double SurveyorAmount { get; set; }

    public double TechnicalAmount { get; set; }

    public double ConcreteLabAmount { get; set; }

    public double GeoTechnicAmount { get; set; }

    public double TaxPercentage { get; set; }

    public double TollPercentage { get; set; }

    public virtual BuildingSubGroup BuildingSubGroup { get; set; } = null!;

    public virtual EngServiceTariff EngServiceTariff { get; set; } = null!;

    public virtual ICollection<EngServiceTariffDetailItem> EngServiceTariffDetailItems { get; set; } = new List<EngServiceTariffDetailItem>();
}
