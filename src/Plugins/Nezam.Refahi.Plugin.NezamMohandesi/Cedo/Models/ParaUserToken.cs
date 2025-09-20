namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ParaUserToken
{
    public Guid UserId { get; set; }

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Value { get; set; }

    public virtual ParaUser User { get; set; } = null!;
}
