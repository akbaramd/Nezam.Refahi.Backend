namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ChangePlanFeasibilityJudgement
{
    public int Id { get; set; }

    public int ChangePlanFeasibilityId { get; set; }

    public int? MemberServiceId { get; set; }

    public DateTime RegDate { get; set; }

    public bool IsExSupervisor { get; set; }

    public DateTime? ResultDate { get; set; }

    public bool IsConfirm { get; set; }

    public string? Result { get; set; }

    public int? ResultFileId { get; set; }

    public double? BuildingProgress { get; set; }

    public virtual ChangePlanFeasibility ChangePlanFeasibility { get; set; } = null!;

    public virtual MemberService? MemberService { get; set; }

    public virtual ConstructionLicenseDocument? ResultFile { get; set; }
}
