namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class IntroductionRequestStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public virtual ICollection<IntroductionRequest> IntroductionRequests { get; set; } = new List<IntroductionRequest>();
}
