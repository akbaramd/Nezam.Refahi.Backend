namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwCapacity
{
    public int MemberId { get; set; }

    public string Title { get; set; } = null!;

    public double? WorkItem { get; set; }

    public double? Metraje { get; set; }

    public int? QuotaYear { get; set; }

    public string Status { get; set; } = null!;

    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }
}
