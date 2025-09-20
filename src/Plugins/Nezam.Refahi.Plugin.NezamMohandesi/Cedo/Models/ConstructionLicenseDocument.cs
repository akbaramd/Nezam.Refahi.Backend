namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ConstructionLicenseDocument
{
    public int Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public int DocumentTypeId { get; set; }

    public string? Description { get; set; }

    public DateTime? RegDate { get; set; }

    public virtual ICollection<ChangePlanFeasibilityJudgement> ChangePlanFeasibilityJudgements { get; set; } = new List<ChangePlanFeasibilityJudgement>();

    public virtual ICollection<ChangeSupervisorJudgment> ChangeSupervisorJudgments { get; set; } = new List<ChangeSupervisorJudgment>();

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual ICollection<ContractExpandation> ContractExpandations { get; set; } = new List<ContractExpandation>();

    public virtual DocumentType DocumentType { get; set; } = null!;

    public virtual ICollection<Examination> Examinations { get; set; } = new List<Examination>();

    public virtual ICollection<ExecuterContractTerminationJudgment> ExecuterContractTerminationJudgments { get; set; } = new List<ExecuterContractTerminationJudgment>();

    public virtual ICollection<PlanControl> PlanControls { get; set; } = new List<PlanControl>();

    public virtual ICollection<SafetyReport> SafetyReports { get; set; } = new List<SafetyReport>();
}
