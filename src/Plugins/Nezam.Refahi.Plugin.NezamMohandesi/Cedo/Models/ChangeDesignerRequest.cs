namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangeDesignerRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public DateTime RegDate { get; set; }

    public int? ServiceFieldId { get; set; }

    public bool IsOwnerRequest { get; set; }

    public int RequestStatusId { get; set; }

    public int? InvolvedMemberId { get; set; }

    public bool HasBuildingCertificate { get; set; }

    public string? Description { get; set; }

    public bool AsBuiltRequest { get; set; }

    public Guid? RequestId { get; set; }

    public string? NewDossierNumber { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual InvolvedMember? InvolvedMember { get; set; }

    public virtual ServiceRequest? Request { get; set; }

    public virtual ChangeDesignerRequestStatus RequestStatus { get; set; } = null!;

    public virtual ServiceField? ServiceField { get; set; }
}
