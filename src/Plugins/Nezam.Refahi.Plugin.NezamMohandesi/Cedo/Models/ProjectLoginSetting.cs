namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ProjectLoginSetting
{
    public int Id { get; set; }

    public int TwoFactorModeId { get; set; }

    public virtual Project IdNavigation { get; set; } = null!;

    public virtual TwoFactorMode TwoFactorMode { get; set; } = null!;
}
