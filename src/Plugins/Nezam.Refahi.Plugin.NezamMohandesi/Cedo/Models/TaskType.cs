namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class TaskType
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public bool IsAutomated { get; set; }

    public int ListOrder { get; set; }

    public string? TaskIconType { get; set; }

    public virtual ICollection<NodeTask> NodeTasks { get; set; } = new List<NodeTask>();
}
