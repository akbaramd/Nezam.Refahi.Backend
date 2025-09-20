namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionStepItemGroup
{
    public int Id { get; set; }

    public int SupervisionStepId { get; set; }

    public int ServiceTypeId { get; set; }

    public string Title { get; set; } = null!;

    public bool IsActive { get; set; }

    public int Order { get; set; }

    public virtual ServiceType ServiceType { get; set; } = null!;

    public virtual SupervisionStep SupervisionStep { get; set; } = null!;

    public virtual ICollection<SupervisionStepItem> SupervisionStepItems { get; set; } = new List<SupervisionStepItem>();
}
