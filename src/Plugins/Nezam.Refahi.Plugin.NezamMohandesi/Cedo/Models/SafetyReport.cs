namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SafetyReport
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int ReporterMemberId { get; set; }

    public int ReportFileId { get; set; }

    public int ReportStatusId { get; set; }

    public DateTime ReportDate { get; set; }

    public string? Comment { get; set; }

    public string? IncpectorComment { get; set; }

    public DateTime? InspectorEndAt { get; set; }

    public string? InspectorEndLocation { get; set; }

    public string? InspectorExternalId { get; set; }

    public string? InspectorName { get; set; }

    public DateTime? InspectorStartAt { get; set; }

    public string? InspectorStartLocation { get; set; }

    public int? ReportResultId { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ConstructionLicenseDocument ReportFile { get; set; } = null!;

    public virtual SafetyReportResult? ReportResult { get; set; }

    public virtual SafetyReportStatus ReportStatus { get; set; } = null!;

    public virtual InvolvedMember ReporterMember { get; set; } = null!;
}
