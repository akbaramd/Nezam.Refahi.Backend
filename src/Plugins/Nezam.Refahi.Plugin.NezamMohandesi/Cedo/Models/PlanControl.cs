namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PlanControl
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public int ConstructionLicenseDocumentId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public int PlanControlStatusId { get; set; }

    public bool IsChangePlanControl { get; set; }

    public virtual ConstructionLicenseDocument ConstructionLicenseDocument { get; set; } = null!;

    public virtual InvolvedMember InvolvedMember { get; set; } = null!;

    public virtual PlanControlStatus PlanControlStatus { get; set; } = null!;
}
