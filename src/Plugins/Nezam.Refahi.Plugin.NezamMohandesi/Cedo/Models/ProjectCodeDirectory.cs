namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ProjectCodeDirectory
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int? ParentDirectoryId { get; set; }

    public int ProjectId { get; set; }

    public virtual ICollection<ProjectCodeDirectory> InverseParentDirectory { get; set; } = new List<ProjectCodeDirectory>();

    public virtual ProjectCodeDirectory? ParentDirectory { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<ProjectCodeFile> ProjectCodeFiles { get; set; } = new List<ProjectCodeFile>();
}
