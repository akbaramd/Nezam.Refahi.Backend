namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ElectricalSpecForm
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public DateTime RegDate { get; set; }

    public int PipeType { get; set; }

    public bool PipeTypeStandard { get; set; }

    public int ExecutionPipeType { get; set; }

    public bool ExecutionPipeTypeStandard { get; set; }

    public int LengthProportion { get; set; }

    public int LightingCircuitsNum { get; set; }

    public int CommonLightingSystem { get; set; }

    public string? OtherCommonLightingSystem { get; set; }

    public bool CommonLightingStandard { get; set; }

    public int OutletCircuitNum { get; set; }

    public int OutletType { get; set; }

    public bool OutletStandard { get; set; }

    public int LightingOutletCircuits { get; set; }

    public int GroundSystem { get; set; }

    public int SparkArrestor { get; set; }

    public bool SparkArrestorStandard { get; set; }

    public int OpenerType { get; set; }

    public int OpenerCabling { get; set; }

    public int FireAlarmSystem { get; set; }

    public bool FireAlarmStandard { get; set; }

    public int GarageOpener { get; set; }

    public bool GarageOpenerStandard { get; set; }

    public int Iphone { get; set; }

    public string? OtherIphone { get; set; }

    public bool IphoneStandard { get; set; }

    public int Tvantenna { get; set; }

    public bool TvantennaStandard { get; set; }

    public int TelephoneSystem { get; set; }

    public int TelPerUnit { get; set; }

    public int AudioSystem { get; set; }

    public int AudioSystemPerUnit { get; set; }

    public string? Cctv { get; set; }

    public int AdditionalEquipment { get; set; }

    public string? OtherAdditionalEquipment { get; set; }

    public bool AdditionalStandard { get; set; }

    public int PowerBranchType { get; set; }

    public string? PhasesNum { get; set; }

    public string? AmpereValue { get; set; }

    public int Generator { get; set; }

    public string? GeneratorPhasesNum { get; set; }

    public bool GeneratorStandard { get; set; }

    public int HazardLight { get; set; }

    public bool HazardLightStandard { get; set; }

    public int LightningArrester { get; set; }

    public bool LightningArresterStandard { get; set; }

    public int ElectricalQuality { get; set; }

    public int StandardElectricalMaterials { get; set; }

    public virtual ICollection<Elevator> Elevators { get; set; } = new List<Elevator>();

    public virtual InvolvedMember InvolvedMember { get; set; } = null!;
}
