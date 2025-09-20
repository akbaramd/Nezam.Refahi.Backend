namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Complainee
{
    public int Id { get; set; }

    public int ComplaintId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? NationalCode { get; set; }

    public string? Address { get; set; }

    public virtual Complaint Complaint { get; set; } = null!;
}
