namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GradeCalculationSetting
{
    public int Id { get; set; }

    public int BuildingGroupSettingId { get; set; }

    public int ServiceTypeId { get; set; }

    public int? ServiceFieldId { get; set; }

    public int MinGradeId { get; set; }

    public virtual BuildingGroupSetting BuildingGroupSetting { get; set; } = null!;

    public virtual EngineeringGrade MinGrade { get; set; } = null!;

    public virtual ServiceField? ServiceField { get; set; }

    public virtual ServiceType ServiceType { get; set; } = null!;
}
