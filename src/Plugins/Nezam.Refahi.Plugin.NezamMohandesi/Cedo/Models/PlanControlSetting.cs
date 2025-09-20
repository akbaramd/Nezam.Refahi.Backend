namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PlanControlSetting
{
    public int Id { get; set; }

    public int SourceCityId { get; set; }

    public int ServiceFieldId { get; set; }

    public int? TargetCityId { get; set; }

    public bool IsActive { get; set; }

    public int? DossierTypeId { get; set; }

    public Guid? DefaultUserIdId { get; set; }

    public int ServiceTypeId { get; set; }

    public int? BuildingSubGroupId { get; set; }

    public virtual BuildingSubGroup? BuildingSubGroup { get; set; }

    public virtual ParaUser? DefaultUserId { get; set; }

    public virtual DossierType? DossierType { get; set; }

    public virtual ServiceField ServiceField { get; set; } = null!;

    public virtual ServiceType ServiceType { get; set; } = null!;

    public virtual City SourceCity { get; set; } = null!;

    public virtual City? TargetCity { get; set; }
}
