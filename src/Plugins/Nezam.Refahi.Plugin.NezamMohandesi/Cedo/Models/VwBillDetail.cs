namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwBillDetail
{
    public int Id { get; set; }

    public int? ConstructionLicenseId { get; set; }

    public string Title { get; set; } = null!;

    public long? Expr1 { get; set; }

    public string? DossierNumber { get; set; }

    public int CostTypeId { get; set; }

    public long Amount { get; set; }

    public long Tax { get; set; }

    public string Expr2 { get; set; } = null!;

    public int Expr3 { get; set; }
}
