namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class TimerEvent
{
    public int Id { get; set; }

    public int TimerTypeId { get; set; }

    public int Year { get; set; }

    public int Month { get; set; }

    public int Day { get; set; }

    public int Hour { get; set; }

    public int Minute { get; set; }

    public int Second { get; set; }

    public virtual FlowEvent IdNavigation { get; set; } = null!;

    public virtual TimerType TimerType { get; set; } = null!;
}
