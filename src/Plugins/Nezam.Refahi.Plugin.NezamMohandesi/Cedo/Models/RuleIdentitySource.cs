namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class RuleIdentitySource
{
    public int Id { get; set; }

    public bool? IsAuthenticated { get; set; }

    public virtual FirewallRule IdNavigation { get; set; } = null!;
}
