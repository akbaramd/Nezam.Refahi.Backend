namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class UserDelegation
{
    public int Id { get; set; }

    public Guid DelegateSenderId { get; set; }

    public Guid DelegateRecieverId { get; set; }

    public DateTime RegDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ParaUser DelegateReciever { get; set; } = null!;

    public virtual ParaUser DelegateSender { get; set; } = null!;
}
