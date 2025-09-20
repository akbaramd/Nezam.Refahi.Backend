namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SmsAccount
{
    public int Id { get; set; }

    public string Provider { get; set; } = null!;

    public string Number { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsDefault { get; set; }

    public virtual ICollection<SendSmsTask> SendSmsTasks { get; set; } = new List<SendSmsTask>();

    public virtual ICollection<SentSmse> SentSmses { get; set; } = new List<SentSmse>();
}
