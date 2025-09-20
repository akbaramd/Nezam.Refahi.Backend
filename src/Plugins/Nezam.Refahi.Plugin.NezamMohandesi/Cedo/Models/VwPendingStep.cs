namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwPendingStep
{
    public Guid UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public int Id { get; set; }

    public int? Code { get; set; }

    public Guid? ActorId { get; set; }

    public int StatusId { get; set; }

    public Guid? OwnerId { get; set; }

    public string Title { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}
