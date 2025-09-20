namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class IncludedResource
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public int Order { get; set; }

    public string Path { get; set; } = null!;

    public int ResourceTypeId { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual IncludedResourceType ResourceType { get; set; } = null!;
}
