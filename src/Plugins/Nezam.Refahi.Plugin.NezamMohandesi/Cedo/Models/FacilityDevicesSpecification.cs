namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FacilityDevicesSpecification
{
    public int Id { get; set; }

    public int DeviceTypeId { get; set; }

    public int FormId { get; set; }

    public float? Count { get; set; }

    public float? Capacity { get; set; }

    public string? Description { get; set; }

    public bool? IsStandard { get; set; }

    public virtual FacilityDeviceType DeviceType { get; set; } = null!;

    public virtual MechanicalSpecForm Form { get; set; } = null!;
}
