namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class FlowNode
{
    public int Id { get; set; }

    public Guid UniqueId { get; set; }

    public string Name { get; set; } = null!;

    public int? Code { get; set; }

    public bool ShowInStepsList { get; set; }

    public int NodeTypeId { get; set; }

    public int DiagramLayoutX { get; set; }

    public int DiagramLayoutY { get; set; }

    public int DiagramLayoutWidth { get; set; }

    public int DiagramLayoutHeight { get; set; }

    public int FlowVersionId { get; set; }

    public DateTime? LastChanged { get; set; }

    public Guid? ActorId { get; set; }

    public string? ForwardScriptCode { get; set; }

    public virtual FlowActor? Actor { get; set; }

    public virtual ICollection<BoundaryEvent> BoundaryEventBoundarySources { get; set; } = new List<BoundaryEvent>();

    public virtual BoundaryEvent? BoundaryEventIdNavigation { get; set; }

    public virtual ICollection<FlowConnector> FlowConnectorSourceNodes { get; set; } = new List<FlowConnector>();

    public virtual ICollection<FlowConnector> FlowConnectorTargetNodes { get; set; } = new List<FlowConnector>();

    public virtual FlowEvent? FlowEvent { get; set; }

    public virtual FlowGatewaie? FlowGatewaie { get; set; }

    public virtual ICollection<FlowNodeLifeCycleEvent> FlowNodeLifeCycleEvents { get; set; } = new List<FlowNodeLifeCycleEvent>();

    public virtual ICollection<FlowStep> FlowSteps { get; set; } = new List<FlowStep>();

    public virtual FlowVersion FlowVersion { get; set; } = null!;

    public virtual NodeAnnotationLayout? NodeAnnotationLayout { get; set; }

    public virtual NodeShapeStyle? NodeShapeStyle { get; set; }

    public virtual NodeTask? NodeTask { get; set; }

    public virtual FlowNodeType NodeType { get; set; } = null!;
}
