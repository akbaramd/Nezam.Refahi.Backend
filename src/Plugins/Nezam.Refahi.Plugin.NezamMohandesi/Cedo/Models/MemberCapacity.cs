namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MemberCapacity
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public float TotalMetraj { get; set; }

    public float RemainingMetraj { get; set; }

    public float RemainingWorkItem { get; set; }

    public float TotalWorkItem { get; set; }

    public int? FieldId { get; set; }

    public DateTime LastSync { get; set; }

    public int CityId { get; set; }

    public int? MaxConcurrent { get; set; }

    public int? ServiceTypeId { get; set; }

    public int Year { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ServiceField? Field { get; set; }

    public virtual Member Member { get; set; } = null!;

    public virtual ServiceType? ServiceType { get; set; }
}
