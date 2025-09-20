namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwNotReleased
{
    public int Id { get; set; }

    public string LicenseNumber { get; set; } = null!;

    public string? DossierNumber { get; set; }

    public int StatusId { get; set; }

    public int StepId { get; set; }

    public bool? IsReleased { get; set; }

    public string? DossierSerial { get; set; }

    public string Name { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Expr1 { get; set; } = null!;
}
