namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwSupervisorContractTempDossier
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? DossierNumber { get; set; }

    public string? DossierSerial { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int StepId { get; set; }

    public int StatusId { get; set; }

    public string Expr1 { get; set; } = null!;

    public string Expr2 { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? NationalCode { get; set; }

    public bool? IsMainPossessor { get; set; }
}
