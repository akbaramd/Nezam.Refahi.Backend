namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ViewComponent
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public DateTime? LastChanged { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<ViewComponentView> ViewComponentViews { get; set; } = new List<ViewComponentView>();
}
