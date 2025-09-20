namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class City
{
    public int Id { get; set; }

    public int ProvinceId { get; set; }

    public string Name { get; set; } = null!;

    public int UtmZone { get; set; }

    public bool Capital { get; set; }

    public int? SyncCode { get; set; }

    public bool IsSatellite { get; set; }

    public bool IsSatelliteConfig { get; set; }

    public virtual ICollection<ActivityLicense> ActivityLicenses { get; set; } = new List<ActivityLicense>();

    public virtual ICollection<ActivityTransferRequest> ActivityTransferRequests { get; set; } = new List<ActivityTransferRequest>();

    public virtual ICollection<BuildingGroupSettingCity> BuildingGroupSettingCities { get; set; } = new List<BuildingGroupSettingCity>();

    public virtual ICollection<CedoAgencyCity> CedoAgencyCities { get; set; } = new List<CedoAgencyCity>();

    public virtual ICollection<ExpertCostsSetting> ExpertCostsSettings { get; set; } = new List<ExpertCostsSetting>();

    public virtual ICollection<GasBranchOffice> GasBranchOffices { get; set; } = new List<GasBranchOffice>();

    public virtual ICollection<GasRequest> GasRequests { get; set; } = new List<GasRequest>();

    public virtual ICollection<MemberCapacity> MemberCapacities { get; set; } = new List<MemberCapacity>();

    public virtual ICollection<MemberService> MemberServices { get; set; } = new List<MemberService>();

    public virtual ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>();

    public virtual ICollection<OnlinePaymentSetting> OnlinePaymentSettings { get; set; } = new List<OnlinePaymentSetting>();

    public virtual ICollection<Operator> Operators { get; set; } = new List<Operator>();

    public virtual ICollection<PlanControlSetting> PlanControlSettingSourceCities { get; set; } = new List<PlanControlSetting>();

    public virtual ICollection<PlanControlSetting> PlanControlSettingTargetCities { get; set; } = new List<PlanControlSetting>();

    public virtual ICollection<ProjectWorkItemSetting> ProjectWorkItemSettings { get; set; } = new List<ProjectWorkItemSetting>();

    public virtual Province Province { get; set; } = null!;

    public virtual ICollection<SeparationPlanRequest> SeparationPlanRequests { get; set; } = new List<SeparationPlanRequest>();

    public virtual ICollection<SubWorkItemSetting> SubWorkItemSettings { get; set; } = new List<SubWorkItemSetting>();

    public virtual ICollection<ViolationReportUser> ViolationReportUsers { get; set; } = new List<ViolationReportUser>();

    public virtual ICollection<WorkItemSetting> WorkItemSettings { get; set; } = new List<WorkItemSetting>();
}
