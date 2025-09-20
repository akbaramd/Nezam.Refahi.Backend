namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class ExecuterContract
{
    public int Id { get; set; }

    public int ContractSubject { get; set; }

    public bool ContractPurpose { get; set; }

    public string? PurposeDescription { get; set; }

    public string? Item21 { get; set; }

    public string? ContractConditions { get; set; }

    public long? ManagerFund { get; set; }

    public long? ManagerFundGurantee { get; set; }

    public double ManagerWage { get; set; }

    public double? ManagerWageGuarantee { get; set; }

    public double? ManagerWagePercent { get; set; }

    public virtual Contract IdNavigation { get; set; } = null!;
}
