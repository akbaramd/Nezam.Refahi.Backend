namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class TwoFactorMode
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ProjectLoginSetting> ProjectLoginSettings { get; set; } = new List<ProjectLoginSetting>();
}
