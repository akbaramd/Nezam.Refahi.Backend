namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class WebPushSetting
{
    public Guid Id { get; set; }

    public virtual ParaUser IdNavigation { get; set; } = null!;

    public virtual ICollection<WebPushSubscription> WebPushSubscriptions { get; set; } = new List<WebPushSubscription>();
}
