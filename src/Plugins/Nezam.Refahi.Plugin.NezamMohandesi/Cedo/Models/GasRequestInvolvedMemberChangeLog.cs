namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequestInvolvedMemberChangeLog
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string Description { get; set; } = null!;

    public virtual GasRequestInvolvedMember IdNavigation { get; set; } = null!;

    public virtual ParaUser User { get; set; } = null!;
}
