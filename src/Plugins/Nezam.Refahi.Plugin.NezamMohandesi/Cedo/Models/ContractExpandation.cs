namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ContractExpandation
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int ServiceTypeId { get; set; }

    public int? ServiceFieldId { get; set; }

    public DateTime RegDate { get; set; }

    public int Duration { get; set; }

    public DateTime? StartDate { get; set; }

    public int BuildingProgress { get; set; }

    public string? Description { get; set; }

    public int? ConstructionLicenseDocumentId { get; set; }

    public int ContractExpandationStatusId { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ConstructionLicenseDocument? ConstructionLicenseDocument { get; set; }

    public virtual ContractExpandationStatus ContractExpandationStatus { get; set; } = null!;

    public virtual ServiceField? ServiceField { get; set; }

    public virtual ServiceType ServiceType { get; set; } = null!;
}
