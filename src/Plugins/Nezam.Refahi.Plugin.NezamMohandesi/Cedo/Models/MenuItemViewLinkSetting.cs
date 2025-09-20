namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MenuItemViewLinkSetting
{
    public int Id { get; set; }

    public int EntityViewId { get; set; }

    public bool ShowInWindow { get; set; }

    public virtual ProjectView EntityView { get; set; } = null!;

    public virtual MenuItem IdNavigation { get; set; } = null!;
}
