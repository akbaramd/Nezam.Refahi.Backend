namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MenuItem
{
    public int Id { get; set; }

    public int ProjectMenuId { get; set; }

    public int? ParentMenuItemId { get; set; }

    public Guid UniqueId { get; set; }

    public string Title { get; set; } = null!;

    public int Order { get; set; }

    public string? IconClass { get; set; }

    public int ItemType { get; set; }

    public string? Url { get; set; }

    public bool TabOrWindow { get; set; }

    public virtual ICollection<MenuItem> InverseParentMenuItem { get; set; } = new List<MenuItem>();

    public virtual MenuItemExternalLinkSetting? MenuItemExternalLinkSetting { get; set; }

    public virtual MenuItemFlowLinkSetting? MenuItemFlowLinkSetting { get; set; }

    public virtual MenuItemViewLinkSetting? MenuItemViewLinkSetting { get; set; }

    public virtual MenuItem? ParentMenuItem { get; set; }

    public virtual ProjectMenu ProjectMenu { get; set; } = null!;
}
