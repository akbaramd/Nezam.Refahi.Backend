namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Payment
{
    public decimal Amount { get; set; }

    public string? Rrn { get; set; }

    public int ConstructionLicenseId { get; set; }

    public string? DossierNumber { get; set; }

    public string LicenseNumber { get; set; } = null!;

    public int PaymentStatusId { get; set; }
}
