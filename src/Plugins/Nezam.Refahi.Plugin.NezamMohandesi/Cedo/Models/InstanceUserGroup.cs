namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class InstanceUserGroup
{
    public int Id { get; set; }

    public virtual ICollection<FlowInstance> FlowInstances { get; set; } = new List<FlowInstance>();

    public virtual ICollection<InstanceUserGroupMember> InstanceUserGroupMembers { get; set; } = new List<InstanceUserGroupMember>();
}
