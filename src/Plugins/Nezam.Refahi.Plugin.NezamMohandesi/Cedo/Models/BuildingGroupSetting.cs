namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BuildingGroupSetting
{
    public int Id { get; set; }

    public int? MunicipalityId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public bool IsActive { get; set; }

    public int FloorCount { get; set; }

    public int Metraj { get; set; }

    public bool NeedsGeo { get; set; }

    public bool NeedsArchitecturePlan { get; set; }

    public bool NeedsStructurePlan { get; set; }

    public bool NeedsElectricPlan { get; set; }

    public bool NeedsMechanicPlan { get; set; }

    public bool NeedsArchitectureSupervisor { get; set; }

    public bool NeedsStructureSupervisor { get; set; }

    public bool NeedsElectricSupervisor { get; set; }

    public bool NeedsMechanicSupervisor { get; set; }

    public bool NeedsSurveyorSupervisor { get; set; }

    public bool NeedsExecuter { get; set; }

    public bool NeedsInsuranceCheck { get; set; }

    public int ContractDuration { get; set; }

    public int GeoMinArea { get; set; }

    public int GeoMinFloorCount { get; set; }

    public bool ManualSupervisorSelection { get; set; }

    public int MinMetrajNeedsExecuter { get; set; }

    public int MinMetraj { get; set; }

    public int SubGroupId { get; set; }

    public int ArchitechtureContractDuration { get; set; }

    public int ElectricityContractDuration { get; set; }

    public bool IsChangeDesigner { get; set; }

    public int MechanicContractDuration { get; set; }

    public int StructureContractDuration { get; set; }

    public int ExecuterContractDuration { get; set; }

    public int ExecuterMinContractAmount { get; set; }

    public int MinFloorsNeedsExecuter { get; set; }

    public int SupervisorCount { get; set; }

    public bool NeedsMechaniOrElectricity { get; set; }

    public DateTime? ExpireDate { get; set; }

    public int? DossierTypeId { get; set; }

    public virtual ICollection<AsBuiltRequest> AsBuiltRequestFinalBuildingGroups { get; set; } = new List<AsBuiltRequest>();

    public virtual ICollection<AsBuiltRequest> AsBuiltRequestIntialBuildingGroupSettings { get; set; } = new List<AsBuiltRequest>();

    public virtual ICollection<BuildingGroupSettingCity> BuildingGroupSettingCities { get; set; } = new List<BuildingGroupSettingCity>();

    public virtual ICollection<BuildingGroupSettingRatio> BuildingGroupSettingRatios { get; set; } = new List<BuildingGroupSettingRatio>();

    public virtual ICollection<CapacityCalculation> CapacityCalculations { get; set; } = new List<CapacityCalculation>();

    public virtual ICollection<ConstructionLicense> ConstructionLicenseBuildingGroupSettings { get; set; } = new List<ConstructionLicense>();

    public virtual ICollection<ConstructionLicense> ConstructionLicenseFinalBuildingGroupSettings { get; set; } = new List<ConstructionLicense>();

    public virtual DossierType? DossierType { get; set; }

    public virtual ICollection<GradeCalculationSetting> GradeCalculationSettings { get; set; } = new List<GradeCalculationSetting>();

    public virtual Municipality? Municipality { get; set; }

    public virtual BuildingSubGroup SubGroup { get; set; } = null!;
}
