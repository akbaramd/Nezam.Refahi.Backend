namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ParaRoleClaim
{
    public int Id { get; set; }

    public Guid RoleId { get; set; }

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual ParaRole Role { get; set; } = null!;
}
