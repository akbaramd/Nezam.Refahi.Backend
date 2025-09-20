namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasBranchOffice
{
    public int Id { get; set; }

    public int CityId { get; set; }

    public string Title { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Telephone { get; set; } = null!;

    public virtual City City { get; set; } = null!;

    public virtual ICollection<GasBranchUser> GasBranchUsers { get; set; } = new List<GasBranchUser>();
}
