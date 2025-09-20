namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangeExecuterRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public DateTime RegDate { get; set; }

    public string? OwnerDescription { get; set; }

    public string? ExecuterDescription { get; set; }

    public int StatusId { get; set; }

    public double RemainingMetraje { get; set; }

    public double RemainingWorkItem { get; set; }

    public bool AsBuiltRequest { get; set; }

    public Guid? RequestId { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ServiceRequest? Request { get; set; }

    public virtual ChangeExecuterRequestStatus Status { get; set; } = null!;
}
