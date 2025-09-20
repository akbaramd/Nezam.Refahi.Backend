namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasUsageType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<GasRequestAuditForm> GasRequestAuditForms { get; set; } = new List<GasRequestAuditForm>();

    public virtual ICollection<GasRequestSplit> GasRequestSplitFinalUsageTypes { get; set; } = new List<GasRequestSplit>();

    public virtual ICollection<GasRequestSplit> GasRequestSplitUsageTypes { get; set; } = new List<GasRequestSplit>();
}
