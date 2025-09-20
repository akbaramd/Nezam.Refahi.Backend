namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class UserProfile
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public bool? Gender { get; set; }

    public string? NationalCode { get; set; }

    public string? BirthCertId { get; set; }

    public string? Father { get; set; }

    public DateTime? Birthdate { get; set; }

    public DateTime RegDate { get; set; }

    public string? Comment { get; set; }

    public virtual ParaUser IdNavigation { get; set; } = null!;

    public virtual Personel? Personel { get; set; }
}
