namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ArchitectureSpecForm
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public DateTime RegDate { get; set; }

    public int ExteriorWallTypes { get; set; }

    public int ExteriorWallMaterials { get; set; }

    public string? OtherExteriorWallMaterials { get; set; }

    public int InteriorWallTypes { get; set; }

    public int InteriorWallMaterials { get; set; }

    public string? OtherInteriorWallMaterials { get; set; }

    public int ExteriorViews { get; set; }

    public string? OtherExteriorViews { get; set; }

    public int InteriorViews { get; set; }

    public string? OtherInteriorViews { get; set; }

    public int RoofTopCovers { get; set; }

    public string? OtherRoofTopCovers { get; set; }

    public int FloorCovers { get; set; }

    public string? OtherFloorCovers { get; set; }

    public int FalseCeilings { get; set; }

    public string? OtherFalseCeilings { get; set; }

    public int Windows { get; set; }

    public string? OtherWindows { get; set; }

    public int Stairs { get; set; }

    public string? OtherStairs { get; set; }

    public int GlassTypes { get; set; }

    public int GlassMaterials { get; set; }

    public string? OtherGlassTypes { get; set; }

    public int ExteriorWallsWaterproofings { get; set; }

    public string? ExteriorWallsWaterproofingPlace { get; set; }

    public int FloorHeatInsulation { get; set; }

    public string? FloorHeatInsulationPlace { get; set; }

    public int CeilingHeatInsulation { get; set; }

    public string? CeilingHeatInsulationPlace { get; set; }

    public int ExteriorWallHeatInsulation { get; set; }

    public string? ExteriorWallHeatInsulationPlace { get; set; }

    public int EnergyConsumptionGrade { get; set; }

    public int FloorsAccessibilitySystems { get; set; }

    public bool ArchitectureQuality { get; set; }

    public bool StandardArchitectureMaterials { get; set; }

    public virtual InvolvedMember InvolvedMember { get; set; } = null!;
}
