namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class StepPending
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int StatusId { get; set; }

    public Guid? OwnerId { get; set; }
}
