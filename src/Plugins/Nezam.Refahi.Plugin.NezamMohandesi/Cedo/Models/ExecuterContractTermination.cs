namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ExecuterContractTermination
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public string? Description { get; set; }

    public string? ArbitrationDescription { get; set; }

    public bool HasBuildingProgress { get; set; }

    public bool IsOwnerPayer { get; set; }

    public DateTime RegDate { get; set; }

    public int StatusId { get; set; }

    public bool IsOwnerRequest { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ICollection<ExecuterContractTerminationJudgment> ExecuterContractTerminationJudgments { get; set; } = new List<ExecuterContractTerminationJudgment>();

    public virtual ExecuterContractTerminationStatus Status { get; set; } = null!;
}
