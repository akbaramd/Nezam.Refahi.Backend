namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ParaUserClaim
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual ParaUser User { get; set; } = null!;
}
