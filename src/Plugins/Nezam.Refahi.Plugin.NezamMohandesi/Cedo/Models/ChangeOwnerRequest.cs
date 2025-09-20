namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangeOwnerRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public bool IsBeforeCertificate { get; set; }

    public DateTime RegDate { get; set; }

    public int RequestStatusId { get; set; }

    public int? BuildingProgress { get; set; }

    public string? Description { get; set; }

    public string? LicenseNumber { get; set; }

    public string? RefNumber { get; set; }

    public Guid? RequestId { get; set; }

    public virtual ICollection<ChangeOwnerRequestDetail> ChangeOwnerRequestDetails { get; set; } = new List<ChangeOwnerRequestDetail>();

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ServiceRequest? Request { get; set; }

    public virtual ChangeOwnerStatus RequestStatus { get; set; } = null!;
}
