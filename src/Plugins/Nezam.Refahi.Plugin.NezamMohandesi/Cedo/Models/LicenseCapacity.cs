namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class LicenseCapacity
{
    public Guid Id { get; set; }

    public int MemberId { get; set; }

    public int ServiceTypeId { get; set; }

    public int? ServiceFieldId { get; set; }

    public int? BuildingGroupId { get; set; }

    public double? Metraj { get; set; }

    public double? WorkItem { get; set; }

    public int? Concurrency { get; set; }

    public int? Floors { get; set; }

    public DateTime Date { get; set; }

    public string Description { get; set; } = null!;

    public int? PrevYears { get; set; }

    public virtual BuildingGroup? BuildingGroup { get; set; }

    public virtual LicenseCapacityRemaining? LicenseCapacityRemaining { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ServiceField? ServiceField { get; set; }

    public virtual ServiceType ServiceType { get; set; } = null!;
}
