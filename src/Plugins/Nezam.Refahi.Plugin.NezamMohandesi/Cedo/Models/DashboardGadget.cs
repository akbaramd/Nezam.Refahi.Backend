namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DashboardGadget
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int? GadgetViewId { get; set; }

    public int Order { get; set; }

    public int Column { get; set; }

    public int Row { get; set; }

    public int ColumnSpan { get; set; }

    public int RowSpan { get; set; }

    public virtual ProjectView? GadgetView { get; set; }

    public virtual ParaUser User { get; set; } = null!;
}
