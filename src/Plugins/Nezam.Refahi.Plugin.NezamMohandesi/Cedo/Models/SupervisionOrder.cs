namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SupervisionOrder
{
    public Guid Id { get; set; }

    public int ConstructionLicenseId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int TypeId { get; set; }

    public DateTime RegDate { get; set; }

    public int StatusId { get; set; }

    public virtual ConstructionLicense ConstructionLicense { get; set; } = null!;

    public virtual SupervisionOrderStatus Status { get; set; } = null!;

    public virtual ICollection<SupervisionField> SupervisionFields { get; set; } = new List<SupervisionField>();

    public virtual SupervisionOrderType Type { get; set; } = null!;
}
