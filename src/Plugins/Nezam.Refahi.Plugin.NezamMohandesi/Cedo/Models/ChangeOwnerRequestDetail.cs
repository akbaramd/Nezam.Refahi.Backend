namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangeOwnerRequestDetail
{
    public int Id { get; set; }

    public int ChangeOwnerRequestId { get; set; }

    public int OwnerId { get; set; }

    public virtual ChangeOwnerRequest ChangeOwnerRequest { get; set; } = null!;

    public virtual Owner Owner { get; set; } = null!;
}
