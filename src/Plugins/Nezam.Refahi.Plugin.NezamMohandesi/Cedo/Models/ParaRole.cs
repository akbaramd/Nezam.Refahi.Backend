namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ParaRole
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<ParaRoleClaim> ParaRoleClaims { get; set; } = new List<ParaRoleClaim>();

    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

    public virtual ICollection<RuleRoleSource> RuleRoleSources { get; set; } = new List<RuleRoleSource>();

    public virtual ICollection<ParaUser> Users { get; set; } = new List<ParaUser>();
}
