namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ProjectDataSource
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string FriendlyName { get; set; } = null!;

    public string? DataSourceName { get; set; }

    public int DataSourceTypeId { get; set; }

    public string? DefaultPkType { get; set; }

    public virtual ICollection<DataSourceDiagram> DataSourceDiagrams { get; set; } = new List<DataSourceDiagram>();

    public virtual ICollection<DataSourceEntity> DataSourceEntities { get; set; } = new List<DataSourceEntity>();

    public virtual DataSourceType DataSourceType { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual SqlServerDataSourceConfig? SqlServerDataSourceConfig { get; set; }
}
