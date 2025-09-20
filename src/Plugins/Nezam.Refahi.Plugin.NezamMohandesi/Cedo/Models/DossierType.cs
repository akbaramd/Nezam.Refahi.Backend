namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DossierType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<BuildingGroupSetting> BuildingGroupSettings { get; set; } = new List<BuildingGroupSetting>();

    public virtual ICollection<ConstructionLicense> ConstructionLicenses { get; set; } = new List<ConstructionLicense>();

    public virtual ICollection<EngServiceCostScaleSetting> EngServiceCostScaleSettings { get; set; } = new List<EngServiceCostScaleSetting>();

    public virtual ICollection<LicenseInquery> LicenseInqueries { get; set; } = new List<LicenseInquery>();

    public virtual ICollection<PlanControlSetting> PlanControlSettings { get; set; } = new List<PlanControlSetting>();

    public virtual ICollection<ProjectWorkItemSetting> ProjectWorkItemSettings { get; set; } = new List<ProjectWorkItemSetting>();

    public virtual ICollection<SubWorkItemSetting> SubWorkItemSettings { get; set; } = new List<SubWorkItemSetting>();

    public virtual ICollection<WorkItemSetting> WorkItemSettings { get; set; } = new List<WorkItemSetting>();
}
