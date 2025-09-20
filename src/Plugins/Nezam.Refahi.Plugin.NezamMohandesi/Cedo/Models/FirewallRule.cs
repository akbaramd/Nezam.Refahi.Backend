namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FirewallRule
{
    public int Id { get; set; }

    public int FirewallId { get; set; }

    public string Name { get; set; } = null!;

    public string? DenyMessage { get; set; }

    public bool IsActive { get; set; }

    public bool IsAllowed { get; set; }

    public bool IsCustom { get; set; }

    public int Order { get; set; }

    public bool? RedirectToLogin { get; set; }

    public virtual Firewall Firewall { get; set; } = null!;

    public virtual ICollection<RuleDestination> RuleDestinations { get; set; } = new List<RuleDestination>();

    public virtual ICollection<RuleDeviceSource> RuleDeviceSources { get; set; } = new List<RuleDeviceSource>();

    public virtual RuleIdentitySource? RuleIdentitySource { get; set; }

    public virtual ICollection<RuleRoleSource> RuleRoleSources { get; set; } = new List<RuleRoleSource>();

    public virtual ICollection<RuleSubnetSource> RuleSubnetSources { get; set; } = new List<RuleSubnetSource>();

    public virtual ICollection<RuleTimeSource> RuleTimeSources { get; set; } = new List<RuleTimeSource>();

    public virtual ICollection<RuleUserSource> RuleUserSources { get; set; } = new List<RuleUserSource>();
}
