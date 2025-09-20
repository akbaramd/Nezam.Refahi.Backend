namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class RejectedPlanControlsWithArea
{
    public int ConstructionLicenseId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? DossierNumber { get; set; }

    public double? SumArea { get; set; }

    public DateTime RegDate { get; set; }
}
