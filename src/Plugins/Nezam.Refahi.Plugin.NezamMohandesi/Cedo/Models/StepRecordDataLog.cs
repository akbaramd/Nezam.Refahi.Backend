namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class StepRecordDataLog
{
    public int Id { get; set; }

    public string? Log { get; set; }

    public virtual StepRecordLog IdNavigation { get; set; } = null!;
}
