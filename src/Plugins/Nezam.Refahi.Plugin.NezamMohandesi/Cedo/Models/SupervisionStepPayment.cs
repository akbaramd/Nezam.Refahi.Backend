namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionStepPayment
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public int? SupervisionStepId { get; set; }

    public DateTime RegDate { get; set; }

    public DateTime? PayDate { get; set; }

    public long Amount { get; set; }

    public bool IsCoordinatorPayment { get; set; }

    public bool IsExSupervisorConfirmed { get; set; }

    public bool IsNeedIsExSupervisorConfirm { get; set; }

    public string? Description { get; set; }

    public int PaymentStatusId { get; set; }

    public long RemainingAmount { get; set; }

    public double PaymentPercent { get; set; }

    public virtual InvolvedMember InvolvedMember { get; set; } = null!;

    public virtual SupervisionStepPaymentStatus PaymentStatus { get; set; } = null!;

    public virtual SupervisionStep? SupervisionStep { get; set; }
}
