namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Estate
{
    public int Id { get; set; }

    public int MunicipalityId { get; set; }

    public string Code { get; set; } = null!;

    public string MainPlaque { get; set; } = null!;

    public string SecondaryPlaque { get; set; } = null!;

    public string? PiecePlaque { get; set; }

    public string PartPlaque { get; set; } = null!;

    public string? District { get; set; }

    public string Street { get; set; } = null!;

    public string? Alley { get; set; }

    public string? Plaque { get; set; }

    public string? PostalCode { get; set; }

    public DateTime RegDate { get; set; }

    public int OwnershipDocumentTypeId { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<ConstructionLicense> ConstructionLicenses { get; set; } = new List<ConstructionLicense>();

    public virtual Municipality Municipality { get; set; } = null!;

    public virtual OwnershipDocumentType OwnershipDocumentType { get; set; } = null!;
}
