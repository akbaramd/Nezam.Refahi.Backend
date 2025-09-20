namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class IntroductionTarget
{
    public int Id { get; set; }

    public string OrgTitle { get; set; } = null!;

    public string ReceiverDescription { get; set; } = null!;

    public string ReceiverTitle { get; set; } = null!;

    public virtual ICollection<IntroductionRequest> IntroductionRequests { get; set; } = new List<IntroductionRequest>();
}
