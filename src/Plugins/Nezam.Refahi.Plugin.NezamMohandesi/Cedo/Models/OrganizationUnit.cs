namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class OrganizationUnit
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsManagementScope { get; set; }

    public int? ParentId { get; set; }

    public virtual ICollection<OrganizationUnit> InverseParent { get; set; } = new List<OrganizationUnit>();

    public virtual ICollection<OrganizationPost> OrganizationPosts { get; set; } = new List<OrganizationPost>();

    public virtual OrganizationUnit? Parent { get; set; }
}
