namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ExternalReport
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public string? Description { get; set; }

    public int ExternalReportStatusId { get; set; }

    public string? ExternalUser { get; set; }

    public DateTime RegDate { get; set; }

    public string? Source { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ExternalReportStatus ExternalReportStatus { get; set; } = null!;
}
