namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SafetyReportResult
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<SafetyReport> SafetyReports { get; set; } = new List<SafetyReport>();
}
