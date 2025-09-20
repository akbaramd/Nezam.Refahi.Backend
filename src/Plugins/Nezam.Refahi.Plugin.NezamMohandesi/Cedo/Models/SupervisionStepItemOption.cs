namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionStepItemOption
{
    public int Id { get; set; }

    public int SupervisionStepItemId { get; set; }

    public string ItemOption { get; set; } = null!;

    public virtual SupervisionStepItem SupervisionStepItem { get; set; } = null!;
}
