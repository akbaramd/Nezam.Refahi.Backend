namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class VwUserDeligation
{
    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Value { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}
