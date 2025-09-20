namespace Nezam.Refahi.Plugin.NezamMohandesi.Cedo.Models;

public partial class SystemSetting
{
    public int Id { get; set; }

    public bool UseDeveloperExceptionPage { get; set; }

    public bool UseHsts { get; set; }

    public bool UseHttpsRedirection { get; set; }

    public bool UseCors { get; set; }

    public bool LogWebServices { get; set; }

    public int ErrorLogsMaxAge { get; set; }

    public int VisitStatsMaxAge { get; set; }

    public int WebServiceLogsMaxAge { get; set; }

    public string ApiSecretKey { get; set; } = null!;
}
