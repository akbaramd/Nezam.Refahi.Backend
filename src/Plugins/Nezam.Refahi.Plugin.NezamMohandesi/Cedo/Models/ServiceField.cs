namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ServiceField
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<AdditionalSupervisionField> AdditionalSupervisionFields { get; set; } = new List<AdditionalSupervisionField>();

    public virtual ICollection<BuildingGroupSettingRatio> BuildingGroupSettingRatios { get; set; } = new List<BuildingGroupSettingRatio>();

    public virtual ICollection<CapacityCalculation> CapacityCalculations { get; set; } = new List<CapacityCalculation>();

    public virtual ICollection<ChangeDesignerRequest> ChangeDesignerRequests { get; set; } = new List<ChangeDesignerRequest>();

    public virtual ICollection<ContractExpandation> ContractExpandations { get; set; } = new List<ContractExpandation>();

    public virtual ICollection<CostTypeDetail> CostTypeDetails { get; set; } = new List<CostTypeDetail>();

    public virtual ICollection<CourseQualifiication> CourseQualifiications { get; set; } = new List<CourseQualifiication>();

    public virtual ICollection<ExecuterCompetenceSetting> ExecuterCompetenceSettings { get; set; } = new List<ExecuterCompetenceSetting>();

    public virtual ICollection<FiscalBalance> FiscalBalances { get; set; } = new List<FiscalBalance>();

    public virtual ICollection<GradeCalculationSetting> GradeCalculationSettings { get; set; } = new List<GradeCalculationSetting>();

    public virtual ICollection<LicenseCapacity> LicenseCapacities { get; set; } = new List<LicenseCapacity>();

    public virtual ICollection<MemberCapacity> MemberCapacities { get; set; } = new List<MemberCapacity>();

    public virtual ICollection<MemberService> MemberServices { get; set; } = new List<MemberService>();

    public virtual ICollection<MemberWorkField> MemberWorkFields { get; set; } = new List<MemberWorkField>();

    public virtual ICollection<PlanControlSetting> PlanControlSettings { get; set; } = new List<PlanControlSetting>();

    public virtual ICollection<SupervisionField> SupervisionFields { get; set; } = new List<SupervisionField>();

    public virtual ICollection<SupervisionStep> SupervisionSteps { get; set; } = new List<SupervisionStep>();

    public virtual ICollection<ValidMemberService> ValidMemberServiceMemberFields { get; set; } = new List<ValidMemberService>();

    public virtual ICollection<ValidMemberService> ValidMemberServiceServiceFields { get; set; } = new List<ValidMemberService>();
}
