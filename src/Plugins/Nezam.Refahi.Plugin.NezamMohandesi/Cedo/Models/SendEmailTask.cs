namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SendEmailTask
{
    public int Id { get; set; }

    public int? EmailAccountId { get; set; }

    public string ReceiverAddress { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public virtual EmailAccount? EmailAccount { get; set; }

    public virtual NodeTask IdNavigation { get; set; } = null!;
}
