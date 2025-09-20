namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwContractDossier
{
    public int Id { get; set; }

    public int EstateId { get; set; }

    public string LicenseNumber { get; set; } = null!;

    public string? DossierNumber { get; set; }

    public int StatusId { get; set; }

    public int StepId { get; set; }

    public string? DossierSerial { get; set; }

    public string Title { get; set; } = null!;

    public string Expr1 { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Expr2 { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int OwnershipDocumentTypeId { get; set; }

    public string Expr3 { get; set; } = null!;
}
