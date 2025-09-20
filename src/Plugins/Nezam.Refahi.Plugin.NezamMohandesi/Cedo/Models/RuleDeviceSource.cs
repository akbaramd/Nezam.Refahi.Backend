namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class RuleDeviceSource
{
    public int Id { get; set; }

    public int RuleId { get; set; }

    public string? UserAgentFamily { get; set; }

    public string? OsFamily { get; set; }

    public string? DeviceFamily { get; set; }

    public bool IsNot { get; set; }

    public bool? Mobile { get; set; }

    public virtual FirewallRule Rule { get; set; } = null!;
}
