namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class BuildingCertificateTimeout
{
    public int Id { get; set; }

    public int CheckDuration { get; set; }

    public bool IsActive { get; set; }

    public DateTime? NextCheckDate { get; set; }
}
