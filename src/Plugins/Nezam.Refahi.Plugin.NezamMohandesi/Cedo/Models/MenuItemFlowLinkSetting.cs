namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MenuItemFlowLinkSetting
{
    public int Id { get; set; }

    public int FlowId { get; set; }

    public virtual Flow Flow { get; set; } = null!;

    public virtual MenuItem IdNavigation { get; set; } = null!;
}
