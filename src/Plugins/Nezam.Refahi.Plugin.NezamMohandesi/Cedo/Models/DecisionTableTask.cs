namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DecisionTableTask
{
    public int Id { get; set; }

    public string? TableName { get; set; }

    public string? Script { get; set; }

    public string? DateScript { get; set; }

    public virtual NodeTask IdNavigation { get; set; } = null!;
}
