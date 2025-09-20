namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ScriptExecuteTask
{
    public int Id { get; set; }

    public string? Script { get; set; }

    public virtual NodeTask IdNavigation { get; set; } = null!;
}
