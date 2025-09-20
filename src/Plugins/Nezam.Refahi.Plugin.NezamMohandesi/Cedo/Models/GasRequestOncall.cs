namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequestOncall
{
    public string ReporterName { get; set; } = null!;

    public string? ReporterMobile { get; set; }

    public bool IsRental { get; set; }

    public bool GasLeakage { get; set; }

    public bool Monoxide { get; set; }

    public bool Fire { get; set; }

    public bool UnauthorizedDevelopment { get; set; }

    public bool DocumentAbsence { get; set; }

    public bool GasFlowIntruption { get; set; }

    public string? OtherReasons { get; set; }

    public DateTime CutDate { get; set; }

    public int CounterNumber { get; set; }

    public bool IsConnected { get; set; }

    public int Id { get; set; }

    public string OwnerMobile { get; set; } = null!;

    public string OwnerName { get; set; } = null!;

    public Guid ServiceRequestId { get; set; }

    public string Subscription { get; set; } = null!;

    public string TrackCode { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int StatusId { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<GasRequestInvolvedMember> GasRequestInvolvedMembers { get; set; } = new List<GasRequestInvolvedMember>();

    public virtual ICollection<GasRequestOncallDescription> GasRequestOncallDescriptions { get; set; } = new List<GasRequestOncallDescription>();

    public virtual ServiceRequest ServiceRequest { get; set; } = null!;

    public virtual GasOncallStatus Status { get; set; } = null!;
}
