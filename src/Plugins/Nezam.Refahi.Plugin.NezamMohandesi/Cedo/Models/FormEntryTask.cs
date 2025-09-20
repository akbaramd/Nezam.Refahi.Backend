namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FormEntryTask
{
    public int Id { get; set; }

    public int? AddViewId { get; set; }

    public string? AddUrl { get; set; }

    public int? ViewRecordViewId { get; set; }

    public string? ViewRecordUrl { get; set; }

    public int? EditViewId { get; set; }

    public string? EditUrl { get; set; }

    public string? UrlArgumentsScript { get; set; }

    public bool SingleRecord { get; set; }

    public virtual ProjectView? AddView { get; set; }

    public virtual ProjectView? EditView { get; set; }

    public virtual NodeTask IdNavigation { get; set; } = null!;

    public virtual ProjectView? ViewRecordView { get; set; }
}
