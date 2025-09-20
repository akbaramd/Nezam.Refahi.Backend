namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class StructureSpecForm
{
    public int Id { get; set; }

    public bool Item1 { get; set; }

    public bool Item2 { get; set; }

    public bool Item3 { get; set; }

    public bool Item4 { get; set; }

    public bool Item5 { get; set; }

    public bool Item6 { get; set; }

    public bool Item7 { get; set; }

    public bool Item8 { get; set; }

    public bool Item9 { get; set; }

    public bool Item10 { get; set; }

    public bool Item11 { get; set; }

    public bool Item12 { get; set; }

    public bool Item13 { get; set; }

    public bool Item14 { get; set; }

    public int BuildingGroupImportance { get; set; }

    public int SoilType { get; set; }

    public int FoundationType { get; set; }

    public int StructuralMaterialsType { get; set; }

    public string? OtherStructuralMaterialsType { get; set; }

    public int StructuralType { get; set; }

    public string? OtherStructuralType { get; set; }

    public int RoofType { get; set; }

    public string? OtherRoofType { get; set; }

    public int BasementRetainingWall { get; set; }

    public bool SoilMechanicTest { get; set; }

    public bool SoilMechanicTestResult { get; set; }

    public bool ConcreteSampleTest { get; set; }

    public bool ConcreteSampleTestResult { get; set; }

    public bool WeldingTest { get; set; }

    public bool WeldingTestResult { get; set; }

    public int BearingWallSystem { get; set; }

    public int SimpleBuildingFrameSystem { get; set; }

    public int BendingFrameSystem { get; set; }

    public int DualOrCombinedSystem { get; set; }

    public int HorizontalTies { get; set; }

    public int VerticalTies { get; set; }

    public bool StructureQuality { get; set; }

    public bool StandardStructuralMaterials { get; set; }

    public int InvolvedMemberId { get; set; }

    public DateTime RegDate { get; set; }

    public virtual InvolvedMember InvolvedMember { get; set; } = null!;
}
