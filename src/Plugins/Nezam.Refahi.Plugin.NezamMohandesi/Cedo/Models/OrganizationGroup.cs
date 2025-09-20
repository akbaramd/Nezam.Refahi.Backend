namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class OrganizationGroup
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<PersonelGroup> PersonelGroups { get; set; } = new List<PersonelGroup>();
}
