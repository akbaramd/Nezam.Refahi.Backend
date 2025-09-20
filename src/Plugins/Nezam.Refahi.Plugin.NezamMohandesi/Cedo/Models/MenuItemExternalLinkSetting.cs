namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MenuItemExternalLinkSetting
{
    public int Id { get; set; }

    public string? Url { get; set; }

    public bool ShowInNewTab { get; set; }

    public virtual MenuItem IdNavigation { get; set; } = null!;
}
