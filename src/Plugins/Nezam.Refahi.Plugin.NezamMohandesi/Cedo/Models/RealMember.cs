namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class RealMember
{
    public int Id { get; set; }

    public int HealthStatusId { get; set; }

    public virtual HealthStatus HealthStatus { get; set; } = null!;

    public virtual Member IdNavigation { get; set; } = null!;
}
