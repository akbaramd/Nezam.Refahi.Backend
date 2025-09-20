namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasBranchUser
{
    public Guid Id { get; set; }

    public int BranchId { get; set; }

    public virtual GasBranchOffice Branch { get; set; } = null!;

    public virtual ParaUser IdNavigation { get; set; } = null!;
}
