namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FacilityRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public Guid RequesterId { get; set; }

    public string? Description { get; set; }

    public bool FacilityForWorkItem { get; set; }

    public DateTime RegDate { get; set; }

    public int RequestForId { get; set; }

    public int StatusId { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual FacilityRequestFor RequestFor { get; set; } = null!;

    public virtual ParaUser Requester { get; set; } = null!;

    public virtual FacilityRequestStatus Status { get; set; } = null!;
}
