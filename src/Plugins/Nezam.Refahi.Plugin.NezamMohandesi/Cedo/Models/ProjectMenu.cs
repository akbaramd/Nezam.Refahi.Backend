namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ProjectMenu
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public Guid UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public int Order { get; set; }

    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
}
