namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ServiceRequest
{
    public Guid Id { get; set; }

    public int RequestTypeId { get; set; }

    public Guid? UserId { get; set; }

    public DateTime RegDate { get; set; }

    public int RequestStatusId { get; set; }

    public virtual ICollection<ChangeDesignerRequest> ChangeDesignerRequests { get; set; } = new List<ChangeDesignerRequest>();

    public virtual ICollection<ChangeExecuterRequest> ChangeExecuterRequests { get; set; } = new List<ChangeExecuterRequest>();

    public virtual ICollection<ChangeOwnerRequest> ChangeOwnerRequests { get; set; } = new List<ChangeOwnerRequest>();

    public virtual ICollection<ChangePlanRequest> ChangePlanRequests { get; set; } = new List<ChangePlanRequest>();

    public virtual ICollection<ChangeSupervisorRequest> ChangeSupervisorRequests { get; set; } = new List<ChangeSupervisorRequest>();

    public virtual ICollection<GasRequestOncall> GasRequestOncalls { get; set; } = new List<GasRequestOncall>();

    public virtual ICollection<GasRequest> GasRequests { get; set; } = new List<GasRequest>();

    public virtual RequestStatus RequestStatus { get; set; } = null!;

    public virtual RequestType RequestType { get; set; } = null!;

    public virtual ICollection<SeparationPlanRequest> SeparationPlanRequests { get; set; } = new List<SeparationPlanRequest>();

    public virtual ParaUser? User { get; set; }
}
