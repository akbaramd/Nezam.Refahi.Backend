namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRotatingForm
{
    public int Id { get; set; }

    public int InvolvedMemberId { get; set; }

    public DateTime RegDate { get; set; }

    public bool HasSubscription { get; set; }

    public int SubscriptionTypeId { get; set; }

    public string FlagSerial { get; set; } = null!;

    public bool NeedsRegulator { get; set; }

    public string ValveType { get; set; } = null!;

    public bool PlanType { get; set; }

    public string? PrevSubscription { get; set; }

    public virtual GasRequestInvolvedMember InvolvedMember { get; set; } = null!;

    public virtual GasSubscriptionType SubscriptionType { get; set; } = null!;
}
