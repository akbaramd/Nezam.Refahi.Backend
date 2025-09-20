namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ControllerActionType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ControllerAction> ControllerActions { get; set; } = new List<ControllerAction>();
}
