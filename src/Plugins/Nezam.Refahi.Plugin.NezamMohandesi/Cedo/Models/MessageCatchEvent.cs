namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MessageCatchEvent
{
    public int Id { get; set; }

    public string MessageName { get; set; } = null!;

    public virtual FlowEvent IdNavigation { get; set; } = null!;
}
