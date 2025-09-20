namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwInvolvedMember
{
    public int Expr1 { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int InvolvedmemberId { get; set; }

    public int? SyncCode { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? DossierNumber { get; set; }

    public string Expr2 { get; set; } = null!;

    public string ServiceTypes { get; set; } = null!;

    public int ServiceTypeId { get; set; }

    public string Title { get; set; } = null!;

    public int? ServiceFieldId { get; set; }

    public DateTime RegDate { get; set; }

    public int StatusId { get; set; }

    public int MemberId { get; set; }

    public string Expr3 { get; set; } = null!;

    public int MemberServiceId { get; set; }

    public double? Metraje { get; set; }
}
