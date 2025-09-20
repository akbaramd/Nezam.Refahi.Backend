namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DataSourceEntity
{
    public int Id { get; set; }

    public int ProjectDataSourceId { get; set; }

    public string Name { get; set; } = null!;

    public string EntityName { get; set; } = null!;

    public string? TableName { get; set; }

    public bool IsEnum { get; set; }

    public string? EntityDisplayCode { get; set; }

    public bool IsView { get; set; }

    public DateTime? LastChanged { get; set; }

    public bool GenerateDataServices { get; set; }

    public virtual ICollection<Controller> Controllers { get; set; } = new List<Controller>();

    public virtual ICollection<DiagramEntity> DiagramEntities { get; set; } = new List<DiagramEntity>();

    public virtual ICollection<EntityProperty> EntityProperties { get; set; } = new List<EntityProperty>();

    public virtual ICollection<EnumEntityItem> EnumEntityItems { get; set; } = new List<EnumEntityItem>();

    public virtual ProjectDataSource ProjectDataSource { get; set; } = null!;
}
