namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ExternalReportStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ExternalReport> ExternalReports { get; set; } = new List<ExternalReport>();
}
