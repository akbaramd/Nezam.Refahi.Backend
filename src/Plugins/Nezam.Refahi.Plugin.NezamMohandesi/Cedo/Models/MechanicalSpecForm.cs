namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MechanicalSpecForm
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public DateTime RegDate { get; set; }

    public int WaterPipesType { get; set; }

    public bool WaterPipesStandard { get; set; }

    public int SewerPipesType { get; set; }

    public bool SewerPipesStandard { get; set; }

    public int HeatingPipesType { get; set; }

    public bool HeatingPipesStandard { get; set; }

    public int GasPipesType { get; set; }

    public bool GasPipesStandard { get; set; }

    public int Topic19Observance { get; set; }

    public int EconomicalBuildingGroup { get; set; }

    public int FuelType { get; set; }

    public bool FuelStandard { get; set; }

    public int HeatingSystem { get; set; }

    public bool HeatingSystemStandard { get; set; }

    public int CoolingSystem { get; set; }

    public bool CoolingSystemStandard { get; set; }

    public int HeatColdTransmissionSystem { get; set; }

    public bool HeatColdTransmissionStandard { get; set; }

    public int SewageDisposalSystem { get; set; }

    public bool SewageDisposalStandard { get; set; }

    public int FireSystem { get; set; }

    public bool FireSystemStandard { get; set; }

    public int ElevatorSystem { get; set; }

    public bool ElevatorSystemStandard { get; set; }

    public int AuxiliaryFacilities { get; set; }

    public string? OtherWaterPipesType { get; set; }

    public string? OtherSewerPipesType { get; set; }

    public string? OtherHeatingPipesType { get; set; }

    public string? OtherFuelType { get; set; }

    public string? OtherHeatingSystem { get; set; }

    public string? OtherCoolingSystem { get; set; }

    public string? OtherSewageDisposalSystem { get; set; }

    public string? OtherFireSystem { get; set; }

    public bool MechanicalQuality { get; set; }

    public bool StandardMechanicalMaterials { get; set; }

    public virtual ICollection<FacilityDevicesSpecification> FacilityDevicesSpecifications { get; set; } = new List<FacilityDevicesSpecification>();

    public virtual InvolvedMember InvolvedMember { get; set; } = null!;
}
