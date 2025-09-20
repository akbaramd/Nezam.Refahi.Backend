namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class IncludedResourceType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<IncludedResource> IncludedResources { get; set; } = new List<IncludedResource>();
}
