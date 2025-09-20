namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ViolationReportSubjectType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ViolationReportDetail> ViolationReportDetails { get; set; } = new List<ViolationReportDetail>();
}
