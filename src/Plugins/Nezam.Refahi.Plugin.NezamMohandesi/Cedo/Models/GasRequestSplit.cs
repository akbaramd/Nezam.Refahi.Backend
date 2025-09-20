namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequestSplit
{
    public int Id { get; set; }

    public int GasRequestId { get; set; }

    public int UsageTypeId { get; set; }

    public int CounterTypeId { get; set; }

    public double Usage { get; set; }

    public int Area { get; set; }

    public int Units { get; set; }

    public int Floors { get; set; }

    public string? OwnerName { get; set; }

    public string? OwnerNationalCode { get; set; }

    public string? ZipCode { get; set; }

    public string? OwnerMobile { get; set; }

    public bool BuiltIn { get; set; }

    public string? Subscription { get; set; }

    public int? FinalCounterTypeId { get; set; }

    public int? FinalUnits { get; set; }

    public int? FinalUsageTypeId { get; set; }

    public int Counters { get; set; }

    public int Order { get; set; }

    public virtual GasCounterType CounterType { get; set; } = null!;

    public virtual GasCounterType? FinalCounterType { get; set; }

    public virtual GasUsageType? FinalUsageType { get; set; }

    public virtual GasRequest GasRequest { get; set; } = null!;

    public virtual GasRequestAuditForm? GasRequestAuditForm { get; set; }

    public virtual ICollection<GasRequestSplitDocument> GasRequestSplitDocuments { get; set; } = new List<GasRequestSplitDocument>();

    public virtual ICollection<GasRequestSplitProblem> GasRequestSplitProblems { get; set; } = new List<GasRequestSplitProblem>();

    public virtual GasUsageType UsageType { get; set; } = null!;
}
