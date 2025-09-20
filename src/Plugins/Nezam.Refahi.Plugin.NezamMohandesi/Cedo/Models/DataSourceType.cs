namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class DataSourceType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ProjectDataSource> ProjectDataSources { get; set; } = new List<ProjectDataSource>();
}
