namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class PropertyAttribute
{
    public int Id { get; set; }

    public int EntityPropertyId { get; set; }

    public string AttributeType { get; set; } = null!;

    public DateTime? LastChanged { get; set; }

    public virtual ICollection<AttributePropertyValue> AttributePropertyValues { get; set; } = new List<AttributePropertyValue>();

    public virtual EntityProperty EntityProperty { get; set; } = null!;
}
