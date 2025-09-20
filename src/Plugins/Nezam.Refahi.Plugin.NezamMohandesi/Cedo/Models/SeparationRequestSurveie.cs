namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SeparationRequestSurveie
{
    public int Id { get; set; }

    public int SeparationRequestId { get; set; }

    public int InvolvedMemberId { get; set; }

    public double Metrage { get; set; }

    public int Roofs { get; set; }

    public DateTime SurveyDate { get; set; }

    public bool MunicipalityResponse { get; set; }

    public bool IsCommercial { get; set; }

    public DateTime? SurveyUpdateDate { get; set; }

    public virtual SeparationInvolvedMember InvolvedMember { get; set; } = null!;

    public virtual SeparationPlanRequest SeparationRequest { get; set; } = null!;
}
