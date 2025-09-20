namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DescriptionType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<ConstructionLicenseDescription> ConstructionLicenseDescriptions { get; set; } = new List<ConstructionLicenseDescription>();
}
