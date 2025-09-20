namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class AsBuiltRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public float FirstMetraje { get; set; }

    public float CertificateMetraje { get; set; }

    public float FinalMetraje { get; set; }

    public float AsBuiltMetraje { get; set; }

    public int? IntialBuildingGroupSettingId { get; set; }

    public int? FinalBuildingGroupId { get; set; }

    public int IntialFloorCount { get; set; }

    public int FinalFloorCount { get; set; }

    public string? Description { get; set; }

    public int StatusIdId { get; set; }

    public string? DossierNumber { get; set; }

    public DateTime RegDate { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual BuildingGroupSetting? FinalBuildingGroup { get; set; }

    public virtual BuildingGroupSetting? IntialBuildingGroupSetting { get; set; }

    public virtual AsBuiltRequestStatus StatusId { get; set; } = null!;
}
