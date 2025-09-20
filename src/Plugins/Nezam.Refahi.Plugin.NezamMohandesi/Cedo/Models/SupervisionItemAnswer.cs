namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionItemAnswer
{
    public int Id { get; set; }

    public int ItemId { get; set; }

    public int FormId { get; set; }

    public string? Answer { get; set; }

    public virtual SupervisionStepForm Form { get; set; } = null!;

    public virtual SupervisionStepItem Item { get; set; } = null!;
}
