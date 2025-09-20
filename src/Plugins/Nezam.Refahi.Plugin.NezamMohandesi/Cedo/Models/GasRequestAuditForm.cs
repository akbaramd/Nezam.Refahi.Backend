namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class GasRequestAuditForm
{
    public int Id { get; set; }

    public bool ExecutedBeforeAudit { get; set; }

    public bool HasFlag { get; set; }

    public bool RequiresConsentLetter { get; set; }

    public bool RequiresLicense { get; set; }

    public int UsageTypeId { get; set; }

    public int CounterTypeId { get; set; }

    public bool Gmlocated { get; set; }

    public string? GmlocationDescription { get; set; }

    public bool HatchLocated { get; set; }

    public string? HatchDimention { get; set; }

    public int HatchCount { get; set; }

    public bool AirtightSeam { get; set; }

    public bool NormalSeam { get; set; }

    public bool NeedsSupportFounding { get; set; }

    public bool NeedsSupportClamp { get; set; }

    public string? SupportDescription { get; set; }

    public bool PipelineRounting { get; set; }

    public int Gccount { get; set; }

    public double Gcusage { get; set; }

    public int Hcount { get; set; }

    public double Husage { get; set; }

    public double Hchimney { get; set; }

    public int FireplaceCount { get; set; }

    public double FireplaceUsage { get; set; }

    public double FireplaceChimney { get; set; }

    public int Rccount { get; set; }

    public double Rcusage { get; set; }

    public int WhgroundCount { get; set; }

    public double WhgroundUsage { get; set; }

    public double WhgroundChimney { get; set; }

    public int WhwallCount { get; set; }

    public double WhwallUsage { get; set; }

    public double WhwallChimney { get; set; }

    public int PackageCount { get; set; }

    public double PackageUsage { get; set; }

    public double PackageChimney { get; set; }

    public int Licount { get; set; }

    public double Liusage { get; set; }

    public string? ChimneyOrder { get; set; }

    public bool ChimneyHeight { get; set; }

    public bool Earthquake { get; set; }

    public bool GasFuse { get; set; }

    public bool GasLeakageAlert { get; set; }

    public DateTime RegDate { get; set; }

    public int Units { get; set; }

    public string? Description { get; set; }

    public virtual GasCounterType CounterType { get; set; } = null!;

    public virtual GasRequestSplit IdNavigation { get; set; } = null!;

    public virtual GasUsageType UsageType { get; set; } = null!;
}
