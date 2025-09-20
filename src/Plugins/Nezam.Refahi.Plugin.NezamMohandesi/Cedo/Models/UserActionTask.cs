namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class UserActionTask
{
    public int Id { get; set; }

    public int? ViewId { get; set; }

    public string? ViewUrl { get; set; }

    public string? UrlArgumentsScript { get; set; }

    public bool? RequireDescription { get; set; }

    public virtual NodeTask IdNavigation { get; set; } = null!;

    public virtual ICollection<UserActionTaskAction> UserActionTaskActions { get; set; } = new List<UserActionTaskAction>();

    public virtual ProjectView? View { get; set; }
}
