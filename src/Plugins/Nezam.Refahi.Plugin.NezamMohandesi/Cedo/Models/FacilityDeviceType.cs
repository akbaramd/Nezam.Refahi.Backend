namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FacilityDeviceType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<FacilityDevicesSpecification> FacilityDevicesSpecifications { get; set; } = new List<FacilityDevicesSpecification>();
}
