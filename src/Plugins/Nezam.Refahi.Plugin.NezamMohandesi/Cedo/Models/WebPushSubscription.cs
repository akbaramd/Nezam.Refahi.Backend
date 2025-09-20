namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class WebPushSubscription
{
    public int Id { get; set; }

    public Guid WebPushSettingId { get; set; }

    public string EndPoint { get; set; } = null!;

    public string? Auth { get; set; }

    public string? P256dh { get; set; }

    public DateTime? ExpirationTime { get; set; }

    public virtual WebPushSetting WebPushSetting { get; set; } = null!;
}
