namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class UserActionTaskAction
{
    public int Id { get; set; }

    public Guid UniqueId { get; set; }

    public string Title { get; set; } = null!;

    public int UserActionTaskId { get; set; }

    public int? Code { get; set; }

    public int Order { get; set; }

    public virtual ICollection<TakenUserAction> TakenUserActions { get; set; } = new List<TakenUserAction>();

    public virtual UserActionTask UserActionTask { get; set; } = null!;
}
