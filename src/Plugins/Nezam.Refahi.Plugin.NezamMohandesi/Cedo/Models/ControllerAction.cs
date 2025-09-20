namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ControllerAction
{
    public int Id { get; set; }

    public int ControllerId { get; set; }

    public int ActionTypeId { get; set; }

    public string MethodName { get; set; } = null!;

    public string? ActionName { get; set; }

    public bool Customized { get; set; }

    public DateTime? LastChanged { get; set; }

    public virtual ControllerActionType ActionType { get; set; } = null!;

    public virtual Controller Controller { get; set; } = null!;

    public virtual ControllerActionCode? ControllerActionCode { get; set; }

    public virtual ICollection<PwaPreCacheAction> PwaPreCacheActions { get; set; } = new List<PwaPreCacheAction>();

    public virtual ICollection<PwaShortcutAction> PwaShortcutActions { get; set; } = new List<PwaShortcutAction>();

    public virtual ICollection<ViewAction> ViewActions { get; set; } = new List<ViewAction>();
}
