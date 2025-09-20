namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ProjectWebPushSetting
{
    public int Id { get; set; }

    public string VapidPublicKey { get; set; } = null!;

    public string VapidPrivateKey { get; set; } = null!;

    public string GcmSenderId { get; set; } = null!;

    public string ReplyToAddress { get; set; } = null!;

    public virtual Project IdNavigation { get; set; } = null!;
}
