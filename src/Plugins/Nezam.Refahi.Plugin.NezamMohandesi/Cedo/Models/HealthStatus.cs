namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class HealthStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<RealMember> RealMembers { get; set; } = new List<RealMember>();
}
