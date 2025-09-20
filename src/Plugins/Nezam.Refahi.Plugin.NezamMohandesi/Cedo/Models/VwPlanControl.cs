namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwPlanControl
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public int ConstructionLicenseDocumentId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime RegDate { get; set; }

    public int PlanControlStatusId { get; set; }

    public int DocumentTypeId { get; set; }

    public string Title { get; set; } = null!;

    public int ConstructionLicenseId { get; set; }

    public string Expr1 { get; set; } = null!;
}
