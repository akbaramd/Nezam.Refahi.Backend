namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class NodeTask
{
    public int Id { get; set; }

    public int TaskTypeId { get; set; }

    public virtual DecisionTableTask? DecisionTableTask { get; set; }

    public virtual FlowStarterTask? FlowStarterTask { get; set; }

    public virtual FormEditTask? FormEditTask { get; set; }

    public virtual FormEntryTask? FormEntryTask { get; set; }

    public virtual FlowNode IdNavigation { get; set; } = null!;

    public virtual ScriptExecuteTask? ScriptExecuteTask { get; set; }

    public virtual SendEmailTask? SendEmailTask { get; set; }

    public virtual SendSmsTask? SendSmsTask { get; set; }

    public virtual TaskType TaskType { get; set; } = null!;

    public virtual UserActionTask? UserActionTask { get; set; }
}
