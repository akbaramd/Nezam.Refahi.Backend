namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class MessageThrowEvent
{
    public int Id { get; set; }

    public string? InstanceCode { get; set; }

    public string MessageName { get; set; } = null!;

    public string? VariabelsCode { get; set; }

    public virtual FlowEvent IdNavigation { get; set; } = null!;
}
