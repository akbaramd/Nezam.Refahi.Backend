namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class RuleTimeSource
{
    public int Id { get; set; }

    public int RuleId { get; set; }

    public DateTime? StartDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public DateTime? EndDate { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsNot { get; set; }

    public virtual FirewallRule Rule { get; set; } = null!;
}
