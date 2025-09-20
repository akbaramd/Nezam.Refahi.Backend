namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Flow
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Guid UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public string? RouteParam { get; set; }

    public string? Group { get; set; }

    public virtual ICollection<FlowStarterTask> FlowStarterTasks { get; set; } = new List<FlowStarterTask>();

    public virtual ICollection<FlowVersion> FlowVersions { get; set; } = new List<FlowVersion>();

    public virtual ICollection<MenuItemFlowLinkSetting> MenuItemFlowLinkSettings { get; set; } = new List<MenuItemFlowLinkSetting>();

    public virtual Project Project { get; set; } = null!;
}
