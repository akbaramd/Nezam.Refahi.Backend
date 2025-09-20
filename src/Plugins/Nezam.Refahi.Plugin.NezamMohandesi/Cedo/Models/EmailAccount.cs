namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class EmailAccount
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string SmtpServer { get; set; } = null!;

    public int SmtpServerPort { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string From { get; set; } = null!;

    public bool IsDefault { get; set; }

    public virtual ICollection<SendEmailTask> SendEmailTasks { get; set; } = new List<SendEmailTask>();
}
