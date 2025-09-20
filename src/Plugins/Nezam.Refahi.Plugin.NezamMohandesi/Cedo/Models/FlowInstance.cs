namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowInstance
{
    public int Id { get; set; }

    public int FlowVersionId { get; set; }

    public DateTime RegDate { get; set; }

    public string? TrackCode { get; set; }

    public string? Title { get; set; }

    public int StatusId { get; set; }

    public int UserGroupId { get; set; }

    public int? ParentInstanceId { get; set; }

    public virtual ICollection<FlowInstanceLog> FlowInstanceLogs { get; set; } = new List<FlowInstanceLog>();

    public virtual ICollection<FlowInstancePauseLog> FlowInstancePauseLogs { get; set; } = new List<FlowInstancePauseLog>();

    public virtual ICollection<FlowInstanceVariable> FlowInstanceVariables { get; set; } = new List<FlowInstanceVariable>();

    public virtual ICollection<FlowStep> FlowSteps { get; set; } = new List<FlowStep>();

    public virtual FlowVersion FlowVersion { get; set; } = null!;

    public virtual ICollection<FlowInstance> InverseParentInstance { get; set; } = new List<FlowInstance>();

    public virtual FlowInstance? ParentInstance { get; set; }

    public virtual FlowInstanceStatus Status { get; set; } = null!;

    public virtual InstanceUserGroup UserGroup { get; set; } = null!;
}
