namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DossierCostSetting
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public DateTime RegDate { get; set; }

    public string? BuildingGroup { get; set; }

    public string? FloorCount { get; set; }

    public string? Metraje { get; set; }

    public int? BillTypeId { get; set; }

    public string? Description { get; set; }

    public bool NeedExecuter { get; set; }

    public bool NeedGeoTechnic { get; set; }

    public bool NeedSurveyor { get; set; }

    public bool IsManualSuperVisor { get; set; }

    public virtual BillType? BillType { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ICollection<DossierCostSettingDetail> DossierCostSettingDetails { get; set; } = new List<DossierCostSettingDetail>();
}
