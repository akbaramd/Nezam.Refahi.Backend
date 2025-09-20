namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CancelEvent
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string ConfirmMessage { get; set; } = null!;

    public virtual FlowEvent IdNavigation { get; set; } = null!;
}
