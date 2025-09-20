namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class LicenseCapacityRemaining
{
    public Guid Id { get; set; }

    public double? Metraj { get; set; }

    public double? WorkItem { get; set; }

    public virtual LicenseCapacity IdNavigation { get; set; } = null!;
}
