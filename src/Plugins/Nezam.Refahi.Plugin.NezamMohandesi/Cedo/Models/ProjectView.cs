namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ProjectView
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Content { get; set; }

    public string? ViewWidget { get; set; }

    public DateTime LastModified { get; set; }

    public DateTime? LastRequested { get; set; }

    public DateTime? LastDesigned { get; set; }

    public DateTime? LastPublished { get; set; }

    public bool Customized { get; set; }

    public bool IsHomePage { get; set; }

    public int ProjectId { get; set; }

    public int ViewTypeId { get; set; }

    public DateTime? LastChanged { get; set; }

    public string? FileName { get; set; }

    public virtual ICollection<DashboardGadget> DashboardGadgets { get; set; } = new List<DashboardGadget>();

    public virtual ICollection<FormEditTask> FormEditTaskEditViews { get; set; } = new List<FormEditTask>();

    public virtual ICollection<FormEditTask> FormEditTaskViewRecordViews { get; set; } = new List<FormEditTask>();

    public virtual ICollection<FormEntryTask> FormEntryTaskAddViews { get; set; } = new List<FormEntryTask>();

    public virtual ICollection<FormEntryTask> FormEntryTaskEditViews { get; set; } = new List<FormEntryTask>();

    public virtual ICollection<FormEntryTask> FormEntryTaskViewRecordViews { get; set; } = new List<FormEntryTask>();

    public virtual ICollection<MenuItemViewLinkSetting> MenuItemViewLinkSettings { get; set; } = new List<MenuItemViewLinkSetting>();

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<UserActionTask> UserActionTasks { get; set; } = new List<UserActionTask>();

    public virtual ICollection<ViewAction> ViewActions { get; set; } = new List<ViewAction>();

    public virtual ICollection<ViewComponentView> ViewComponentViews { get; set; } = new List<ViewComponentView>();

    public virtual ViewType ViewType { get; set; } = null!;
}
