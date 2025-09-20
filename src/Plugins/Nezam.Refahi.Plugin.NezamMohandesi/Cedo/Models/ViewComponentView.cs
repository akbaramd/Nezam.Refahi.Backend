namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ViewComponentView
{
    public int Id { get; set; }

    public int ViewComponentId { get; set; }

    public int ViewId { get; set; }

    public virtual ProjectView View { get; set; } = null!;

    public virtual ViewComponent ViewComponent { get; set; } = null!;
}
