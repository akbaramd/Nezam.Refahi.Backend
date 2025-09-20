namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ViolationReportDetail
{
    public int Id { get; set; }

    public int ViolationReportId { get; set; }

    public int MainSubjectId { get; set; }

    public string Subject { get; set; } = null!;

    public string Detail { get; set; } = null!;

    public virtual ViolationReportSubjectType MainSubject { get; set; } = null!;

    public virtual ViolationReport ViolationReport { get; set; } = null!;
}
