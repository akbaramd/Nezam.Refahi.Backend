namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ConstructionLicenseDescription
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public string Description { get; set; } = null!;

    public int DescriptionTypeId { get; set; }

    public DateTime RegDate { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual DescriptionType DescriptionType { get; set; } = null!;
}
