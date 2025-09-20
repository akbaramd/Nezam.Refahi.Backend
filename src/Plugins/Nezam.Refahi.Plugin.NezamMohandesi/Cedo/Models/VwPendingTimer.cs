namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwPendingTimer
{
    public DateTime InvokeAt { get; set; }

    public int StatusId { get; set; }

    public int InstanceStatus { get; set; }

    public string? TrackCode { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentInstanceId { get; set; }

    public int RootInstanceStatus { get; set; }
}
