namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ControllerActionCode
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public DateTime? LastChanged { get; set; }

    public virtual ControllerAction IdNavigation { get; set; } = null!;
}
