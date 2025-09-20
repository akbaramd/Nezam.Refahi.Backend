namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ErrorCatchEvent
{
    public int Id { get; set; }

    public string? ErrorType { get; set; }

    public virtual FlowEvent IdNavigation { get; set; } = null!;
}
