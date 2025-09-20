namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowStarterTask
{
    public int Id { get; set; }

    public int FlowId { get; set; }

    public string? StartupArgScript { get; set; }

    public virtual Flow Flow { get; set; } = null!;

    public virtual NodeTask IdNavigation { get; set; } = null!;
}
