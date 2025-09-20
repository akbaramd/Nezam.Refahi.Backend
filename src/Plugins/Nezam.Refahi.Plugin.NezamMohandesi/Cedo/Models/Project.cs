namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class Project
{
    public int Id { get; set; }

    public string FriendlyName { get; set; } = null!;

    public string RouteParam { get; set; } = null!;

    public string? Name { get; set; }

    public string? Namespace { get; set; }

    public string? DbSchema { get; set; }

    public DateTime? LastDatabaseChange { get; set; }

    public DateTime? LastProjectBuild { get; set; }

    public string? HomePageUrl { get; set; }

    public string? ProjectVersion { get; set; }

    public bool Enabled { get; set; }

    public bool IsRuntime { get; set; }

    public bool Building { get; set; }

    public virtual ICollection<Controller> Controllers { get; set; } = new List<Controller>();

    public virtual ICollection<DecisionTable> DecisionTables { get; set; } = new List<DecisionTable>();

    public virtual ICollection<Flow> Flows { get; set; } = new List<Flow>();

    public virtual ICollection<IncludedResource> IncludedResources { get; set; } = new List<IncludedResource>();

    public virtual ICollection<PageMetaTag> PageMetaTags { get; set; } = new List<PageMetaTag>();

    public virtual ICollection<ProjectCodeDirectory> ProjectCodeDirectories { get; set; } = new List<ProjectCodeDirectory>();

    public virtual ICollection<ProjectDataSource> ProjectDataSources { get; set; } = new List<ProjectDataSource>();

    public virtual ProjectHelp? ProjectHelp { get; set; }

    public virtual ProjectLoginSetting? ProjectLoginSetting { get; set; }

    public virtual ICollection<ProjectMenu> ProjectMenus { get; set; } = new List<ProjectMenu>();

    public virtual ICollection<ProjectRecentDocument> ProjectRecentDocuments { get; set; } = new List<ProjectRecentDocument>();

    public virtual ICollection<ProjectTranslation> ProjectTranslations { get; set; } = new List<ProjectTranslation>();

    public virtual ICollection<ProjectView> ProjectViews { get; set; } = new List<ProjectView>();

    public virtual ProjectWebPushSetting? ProjectWebPushSetting { get; set; }

    public virtual PwaSetting? PwaSetting { get; set; }

    public virtual SolutionResource? SolutionResource { get; set; }

    public virtual ICollection<ViewComponent> ViewComponents { get; set; } = new List<ViewComponent>();

    public virtual ICollection<WebService> WebServices { get; set; } = new List<WebService>();
}
