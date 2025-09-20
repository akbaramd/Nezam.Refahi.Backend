namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowGatewaie
{
    public int Id { get; set; }

    public bool IsExclusive { get; set; }

    public bool WaitForIncomings { get; set; }

    public virtual FlowNode IdNavigation { get; set; } = null!;
}
