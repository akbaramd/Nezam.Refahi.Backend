namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PwaSetting
{
    public int Id { get; set; }

    public bool PwaEnabled { get; set; }

    public string? Name { get; set; }

    public string? ShortName { get; set; }

    public string? Description { get; set; }

    public string? BackgroundColor { get; set; }

    public string? ThemeColor { get; set; }

    public string? Direction { get; set; }

    public string? Display { get; set; }

    public string? Language { get; set; }

    public string? Orientation { get; set; }

    public string? CacheMode { get; set; }

    public virtual Project IdNavigation { get; set; } = null!;

    public virtual ICollection<PwaPreCacheAction> PwaPreCacheActions { get; set; } = new List<PwaPreCacheAction>();

    public virtual ICollection<PwaShortcutAction> PwaShortcutActions { get; set; } = new List<PwaShortcutAction>();
}
