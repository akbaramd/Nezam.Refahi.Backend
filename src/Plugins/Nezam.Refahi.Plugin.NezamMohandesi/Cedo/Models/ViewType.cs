namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ViewType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public bool ForView { get; set; }

    public virtual ICollection<ProjectView> ProjectViews { get; set; } = new List<ProjectView>();
}
