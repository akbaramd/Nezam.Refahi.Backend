namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwCapacityDetail
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int Expr1 { get; set; }

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
}
