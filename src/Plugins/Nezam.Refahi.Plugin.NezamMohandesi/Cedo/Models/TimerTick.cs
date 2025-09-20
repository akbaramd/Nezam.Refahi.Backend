namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class TimerTick
{
    public int Id { get; set; }

    public DateTime InvokeAt { get; set; }

    public virtual FlowStep IdNavigation { get; set; } = null!;
}
