namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DossierBuildingGroupSetting
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public bool NeedGeoTechnic { get; set; }

    public bool NeedElectricityPlan { get; set; }

    public bool NeedMechanicPlan { get; set; }

    public bool NeedExecuter { get; set; }

    public bool NeedsSurveyor { get; set; }

    public string? Description { get; set; }

    public bool IsManualSuperVisor { get; set; }

    public bool IsChangeDesigner { get; set; }

    public int SupervisorCount { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;
}
