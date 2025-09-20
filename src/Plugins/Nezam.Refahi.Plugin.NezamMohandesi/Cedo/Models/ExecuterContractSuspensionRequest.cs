namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ExecuterContractSuspensionRequest
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public DateTime RegDate { get; set; }

    public int Duration { get; set; }

    public string? OwnerDescription { get; set; }

    public string? ExecuterDescription { get; set; }

    public int StatusId { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ExecuterContractSuspensionRequestStatus Status { get; set; } = null!;
}
