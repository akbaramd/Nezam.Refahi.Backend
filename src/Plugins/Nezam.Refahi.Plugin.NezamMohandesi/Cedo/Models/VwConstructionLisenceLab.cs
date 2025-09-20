namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwConstructionLisenceLab
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int LegalMemberTypeId { get; set; }

    public string? DossierNumber { get; set; }

    public int ServiceTypeId { get; set; }

    public int? ServiceFieldId { get; set; }

    public string Expr1 { get; set; } = null!;

    public string Expr2 { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public string Name { get; set; } = null!;

    public int? SyncCode { get; set; }
}
