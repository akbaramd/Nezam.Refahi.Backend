namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Controller
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int ControllerTypeId { get; set; }

    public int? EntityId { get; set; }

    public string? Name { get; set; }

    public string? Area { get; set; }

    public string? PartialCode { get; set; }

    public DateTime? LastChanged { get; set; }

    public virtual ICollection<ControllerAction> ControllerActions { get; set; } = new List<ControllerAction>();

    public virtual ControllerType ControllerType { get; set; } = null!;

    public virtual DataSourceEntity? Entity { get; set; }

    public virtual Project Project { get; set; } = null!;
}
