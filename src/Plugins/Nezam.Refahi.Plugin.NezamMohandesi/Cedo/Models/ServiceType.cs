namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ServiceType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<BuildingGroupSettingRatio> BuildingGroupSettingRatios { get; set; } = new List<BuildingGroupSettingRatio>();

    public virtual ICollection<CapacityCalculation> CapacityCalculations { get; set; } = new List<CapacityCalculation>();

    public virtual ICollection<ContractExpandation> ContractExpandations { get; set; } = new List<ContractExpandation>();

    public virtual ICollection<CourseQualifiication> CourseQualifiications { get; set; } = new List<CourseQualifiication>();

    public virtual ICollection<ExecuterCompetenceSetting> ExecuterCompetenceSettings { get; set; } = new List<ExecuterCompetenceSetting>();

    public virtual ICollection<FiscalBalance> FiscalBalances { get; set; } = new List<FiscalBalance>();

    public virtual ICollection<GradeCalculationSetting> GradeCalculationSettings { get; set; } = new List<GradeCalculationSetting>();

    public virtual ICollection<LicenseCapacity> LicenseCapacities { get; set; } = new List<LicenseCapacity>();

    public virtual ICollection<MemberCapacity> MemberCapacities { get; set; } = new List<MemberCapacity>();

    public virtual ICollection<MemberService> MemberServices { get; set; } = new List<MemberService>();

    public virtual ICollection<PlanControlSetting> PlanControlSettings { get; set; } = new List<PlanControlSetting>();

    public virtual ICollection<SubWorkItemSetting> SubWorkItemSettings { get; set; } = new List<SubWorkItemSetting>();

    public virtual ICollection<SupervisionStepItemGroup> SupervisionStepItemGroups { get; set; } = new List<SupervisionStepItemGroup>();

    public virtual ICollection<ValidMemberService> ValidMemberServices { get; set; } = new List<ValidMemberService>();
}
