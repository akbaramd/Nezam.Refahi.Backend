namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ConstructionLicense
{
    public int Id { get; set; }

    public int EstateId { get; set; }

    public int ConstructionLicenseTypeId { get; set; }

    public int UsageTypeId { get; set; }

    public string RefNumber { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public DateTime LicenseDate { get; set; }

    public DateTime RequestDate { get; set; }

    public int ValidityDuration { get; set; }

    public float BeforeWideningArea { get; set; }

    public float AfterWideningArea { get; set; }

    public float OccupancyPercentage { get; set; }

    public float? DensityPercentage { get; set; }

    public int DeploymentSideId { get; set; }

    public int MaxAllowedFloorGround { get; set; }

    public bool OnGround { get; set; }

    public string? DossierNumber { get; set; }

    public DateTime? DossierDate { get; set; }

    public string? OtherUsageTypes { get; set; }

    public int StatusId { get; set; }

    public int StepId { get; set; }

    public int? ExcavationDangerStatusId { get; set; }

    public int? StructureTypeId { get; set; }

    public bool? HasViolation { get; set; }

    public int MaxAllowedFloorBase { get; set; }

    public int? BuildingGroupSettingId { get; set; }

    public DateTime? LicenseExpireDate { get; set; }

    public string? TrackCode { get; set; }

    public bool? IsManualSuperVisor { get; set; }

    public int? FormStepId { get; set; }

    public string? DossierSerial { get; set; }

    public DateTime? RegDate { get; set; }

    public int DossierTypeId { get; set; }

    public int? ParentId { get; set; }

    public int? MaxConstructionLicenseFloorsCount { get; set; }

    public int? FinalBuildingGroupSettingId { get; set; }

    public bool? IsReleased { get; set; }

    public DateTime? ReleaseDate { get; set; }

    public string? TransitionalDossierNumber { get; set; }

    public string? TransitionalDossierSerial { get; set; }

    public string? DossierFormalCode { get; set; }

    public int? Density { get; set; }

    public string? TransitionalDossierNumber2 { get; set; }

    public string? TransitionalDossierSerial2 { get; set; }

    public virtual ICollection<ArchitectureSafetyForm> ArchitectureSafetyForms { get; set; } = new List<ArchitectureSafetyForm>();

    public virtual ICollection<AsBuiltRequest> AsBuiltRequests { get; set; } = new List<AsBuiltRequest>();

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<BuildingCertificate> BuildingCertificates { get; set; } = new List<BuildingCertificate>();

    public virtual BuildingGroupSetting? BuildingGroupSetting { get; set; }

    public virtual ICollection<CancelDossierRequest> CancelDossierRequests { get; set; } = new List<CancelDossierRequest>();

    public virtual ICollection<ChangeDesignerRequest> ChangeDesignerRequests { get; set; } = new List<ChangeDesignerRequest>();

    public virtual ICollection<ChangeExecuterRequest> ChangeExecuterRequests { get; set; } = new List<ChangeExecuterRequest>();

    public virtual ICollection<ChangeOwnerRequest> ChangeOwnerRequests { get; set; } = new List<ChangeOwnerRequest>();

    public virtual ICollection<ChangePlanRequest> ChangePlanRequests { get; set; } = new List<ChangePlanRequest>();

    public virtual ICollection<ChangeSiteManagerRequest> ChangeSiteManagerRequests { get; set; } = new List<ChangeSiteManagerRequest>();

    public virtual ICollection<ChangeSupervisorRequest> ChangeSupervisorRequests { get; set; } = new List<ChangeSupervisorRequest>();

    public virtual ICollection<ConstructionLicenseComment> ConstructionLicenseComments { get; set; } = new List<ConstructionLicenseComment>();

    public virtual ICollection<ConstructionLicenseDescription> ConstructionLicenseDescriptions { get; set; } = new List<ConstructionLicenseDescription>();

    public virtual ICollection<ConstructionLicenseDocument> ConstructionLicenseDocuments { get; set; } = new List<ConstructionLicenseDocument>();

    public virtual ICollection<ConstructionLicenseGeoLab> ConstructionLicenseGeoLabs { get; set; } = new List<ConstructionLicenseGeoLab>();

    public virtual ICollection<ConstructionLicenseOption> ConstructionLicenseOptions { get; set; } = new List<ConstructionLicenseOption>();

    public virtual ConstructionLicenseType ConstructionLicenseType { get; set; } = null!;

    public virtual ICollection<ContractExpandation> ContractExpandations { get; set; } = new List<ContractExpandation>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual DeploymentSide DeploymentSide { get; set; } = null!;

    public virtual ICollection<DossierBuildingGroupSetting> DossierBuildingGroupSettings { get; set; } = new List<DossierBuildingGroupSetting>();

    public virtual ICollection<DossierCostSetting> DossierCostSettings { get; set; } = new List<DossierCostSetting>();

    public virtual DossierType DossierType { get; set; } = null!;

    public virtual ICollection<ElectricitySafetyForm> ElectricitySafetyForms { get; set; } = new List<ElectricitySafetyForm>();

    public virtual Estate Estate { get; set; } = null!;

    public virtual ICollection<EstateCoordinate> EstateCoordinates { get; set; } = new List<EstateCoordinate>();

    public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();

    public virtual ExcavationDangerStatus? ExcavationDangerStatus { get; set; }

    public virtual ICollection<ExecuterContractSuspensionRequest> ExecuterContractSuspensionRequests { get; set; } = new List<ExecuterContractSuspensionRequest>();

    public virtual ICollection<ExecuterContractTermination> ExecuterContractTerminations { get; set; } = new List<ExecuterContractTermination>();

    public virtual ICollection<ExternalReport> ExternalReports { get; set; } = new List<ExternalReport>();

    public virtual ICollection<FacilityRequest> FacilityRequests { get; set; } = new List<FacilityRequest>();

    public virtual BuildingGroupSetting? FinalBuildingGroupSetting { get; set; }

    public virtual ICollection<Floor> Floors { get; set; } = new List<Floor>();

    public virtual ICollection<ConstructionLicense> InverseParent { get; set; } = new List<ConstructionLicense>();

    public virtual ICollection<InvolvedMember> InvolvedMembers { get; set; } = new List<InvolvedMember>();

    public virtual ICollection<MechanicSafetyForm> MechanicSafetyForms { get; set; } = new List<MechanicSafetyForm>();

    public virtual ICollection<Owner> Owners { get; set; } = new List<Owner>();

    public virtual ConstructionLicense? Parent { get; set; }

    public virtual ICollection<SafetyReport> SafetyReports { get; set; } = new List<SafetyReport>();

    public virtual ICollection<Sgaletter> Sgaletters { get; set; } = new List<Sgaletter>();

    public virtual ConstructionLicenseStatus Status { get; set; } = null!;

    public virtual ConstructionLicenseStep Step { get; set; } = null!;

    public virtual ICollection<StructureSafetyForm> StructureSafetyForms { get; set; } = new List<StructureSafetyForm>();

    public virtual StructureType? StructureType { get; set; }

    public virtual ICollection<SupervisionOrder> SupervisionOrders { get; set; } = new List<SupervisionOrder>();

    public virtual ICollection<Territory> Territories { get; set; } = new List<Territory>();

    public virtual UsageType UsageType { get; set; } = null!;

    public virtual ICollection<ViolationReport> ViolationReports { get; set; } = new List<ViolationReport>();
}
