namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class EntityProperty
{
    public int Id { get; set; }

    public int ProjectEntityId { get; set; }

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? CustomCode { get; set; }

    public DateTime? LastChanged { get; set; }

    public int Order { get; set; }

    public virtual DataSourceEntity ProjectEntity { get; set; } = null!;

    public virtual ICollection<PropertyAttribute> PropertyAttributes { get; set; } = new List<PropertyAttribute>();
}
