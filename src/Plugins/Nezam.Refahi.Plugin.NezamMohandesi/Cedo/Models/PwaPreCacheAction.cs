namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PwaPreCacheAction
{
    public int Id { get; set; }

    public int SettingId { get; set; }

    public int ActionId { get; set; }

    public virtual ControllerAction Action { get; set; } = null!;

    public virtual PwaSetting Setting { get; set; } = null!;
}
