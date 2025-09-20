namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionStepItem
{
    public int Id { get; set; }

    public int GroupId { get; set; }

    public string Title { get; set; } = null!;

    public bool IsActive { get; set; }

    public int Order { get; set; }

    public virtual SupervisionStepItemGroup Group { get; set; } = null!;

    public virtual ICollection<SupervisionItemAnswer> SupervisionItemAnswers { get; set; } = new List<SupervisionItemAnswer>();

    public virtual ICollection<SupervisionStepItemOption> SupervisionStepItemOptions { get; set; } = new List<SupervisionStepItemOption>();
}
