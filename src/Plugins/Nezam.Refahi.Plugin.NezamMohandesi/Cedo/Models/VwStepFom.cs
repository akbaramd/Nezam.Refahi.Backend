namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwStepFom
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public int StepId { get; set; }

    public int? FloorNumber { get; set; }

    public string Title { get; set; } = null!;

    public string Expr1 { get; set; } = null!;

    public string Expr2 { get; set; } = null!;

    public string Expr3 { get; set; } = null!;

    public string Expr4 { get; set; } = null!;

    public string? DossierNumber { get; set; }

    public int Expr5 { get; set; }
}
