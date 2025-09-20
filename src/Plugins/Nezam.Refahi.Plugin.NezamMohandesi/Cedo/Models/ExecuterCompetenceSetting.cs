namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ExecuterCompetenceSetting
{
    public int Id { get; set; }

    public int ServiceFieldId { get; set; }

    public int GradeId { get; set; }

    public int MaxConcurrentMetraje { get; set; }

    public int MaxFloorCount { get; set; }

    public int? MaxMetraje { get; set; }

    public int MemberTypeId { get; set; }

    public bool IsBulkMaker { get; set; }

    public int MaxProjectCount { get; set; }

    public int MemberShipTypeId { get; set; }

    public int ServiceTypeId { get; set; }

    public virtual EngineeringGrade Grade { get; set; } = null!;

    public virtual MemberShipType MemberShipType { get; set; } = null!;

    public virtual MemberType MemberType { get; set; } = null!;

    public virtual ServiceField ServiceField { get; set; } = null!;

    public virtual ServiceType ServiceType { get; set; } = null!;
}
