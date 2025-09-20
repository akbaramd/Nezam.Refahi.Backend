namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class CedoAgency
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Telephone { get; set; } = null!;

    public string Fax { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string WebSite { get; set; } = null!;

    public virtual ICollection<CedoAgencyCity> CedoAgencyCities { get; set; } = new List<CedoAgencyCity>();

    public virtual ICollection<OrgUnit> OrgUnits { get; set; } = new List<OrgUnit>();
}
