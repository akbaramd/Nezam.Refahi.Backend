namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class LogType
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public virtual ICollection<FlowInstanceLog> FlowInstanceLogs { get; set; } = new List<FlowInstanceLog>();
}
